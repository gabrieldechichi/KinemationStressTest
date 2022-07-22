using System.Linq;
using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Entities;
using UnityEngine;

namespace DOTSAnimation.JustSampling
{
    public struct StressTestSampling : IComponentData
    {
        public BlobAssetReference<SkeletonClipSetBlob> Clips;
        public float IdleWeight;
        public float WalkWeight;
        public float NormalizedTime;
    }
    
    [DisallowMultipleComponent]
    public class PureSamplingExampleAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IRequestBlobAssets
    {
        public Animator Animator;
        public AnimationClip Idle;
        public AnimationClip Walk;

        private SmartBlobberHandle<SkeletonClipSetBlob> clipsHandle;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var clips = clipsHandle.Resolve();
            dstManager.AddComponentData(entity, new StressTestSampling()
            {
                Clips = clips,
                IdleWeight = 0.2f,
                WalkWeight = 0.8f
            });
        }

        public void RequestBlobAssets(Entity entity, EntityManager dstEntityManager, GameObjectConversionSystem conversionSystem)
        {
            var clips = new []{Idle, Walk}.Select(c => new SkeletonClipConfig()
            {
                clip = c,
                settings = SkeletonClipCompressionSettings.kDefaultSettings
            });
            clipsHandle = conversionSystem.CreateBlob(Animator.gameObject, new SkeletonClipSetBakeData()
            {
                animator = Animator,
                clips = clips.ToArray()
            });
        }
    }
}
