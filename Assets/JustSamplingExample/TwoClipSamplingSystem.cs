using Latios.Kinemation;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTSAnimation.JustSampling
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(TRSToLocalToParentSystem))]
    [UpdateBefore(typeof(TRSToLocalToWorldSystem))]
    public partial class TwoClipSamplingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;
            Entities.ForEach(
                (ref DynamicBuffer<OptimizedBoneToRoot> boneToRootBuffer,
                ref StressTestSampling clips,
                in OptimizedSkeletonHierarchyBlobReference hierarchyRef) =>
            {

                clips.NormalizedTime += dt;
                var blender = new BufferPoseBlender(boneToRootBuffer);

                ref var idle = ref clips.Clips.Value.clips[0];
                ref var walk = ref clips.Clips.Value.clips[1];

                idle.SamplePose(ref blender, clips.IdleWeight, idle.LoopToClipTime(clips.NormalizedTime));
                walk.SamplePose(ref blender, clips.WalkWeight, walk.LoopToClipTime(clips.NormalizedTime));
                blender.NormalizeRotations();
                blender.ApplyBoneHierarchyAndFinish(hierarchyRef.blob);
            }).ScheduleParallel();
        }
    }
}