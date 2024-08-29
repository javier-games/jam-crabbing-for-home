using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrabAssets.Scripts.Game
{
    [Flags]
    public enum ParticlesType
    {
        None,
        Bubble = 2,
        Fish = 4,
        JellyFish = 8,
        DeadFish = 16,
        DeadWhale = 32
    }
    
    public class ParticlesManager : MonoBehaviour
    {
        [Serializable]
        private struct ParticleSystem
        {
            [SerializeField] public GameObject gameObject;
            [SerializeField] public ParticlesType type;
        }

        [SerializeField] private ParticlesType activeOnAwake;
        [SerializeField] private List<ParticleSystem> systems;

        private void Awake()
        {
            SetActive(activeOnAwake, true);
        }

        private void OnEnable()
        {
            GameTrigger.OnTriggerIn += OnTriggerIn;
            GameTrigger.OnTriggerOut += OnTriggerOut;
        }

        private void OnDisable()
        {
            GameTrigger.OnTriggerIn -= OnTriggerIn;
            GameTrigger.OnTriggerOut -= OnTriggerOut;
        }

        private void OnTriggerIn(GameTrigger trigger, Component actor)
        {
            switch (trigger)
            {
                case ParticlesTrigger particlesTrigger:
                {
                    SetActive(particlesTrigger.ParticleType, true);
                    
                    break;
                }
            }
        }

        private void OnTriggerOut(GameTrigger trigger, Component actor)
        {
            switch (trigger)
            {
                case ParticlesTrigger particlesTrigger:
                {
                    SetActive(particlesTrigger.ParticleType, false);
                    break;
                }
            }
        }

        private void SetActive(ParticlesType type, bool active)
        {
            var mask = (int)type;
            
            for (var i = 0; i < systems.Count; i++)
            {
                var system = systems[i];
                var layer = (int)system.type;
                
                
                if ((mask & layer) > 0)
                {
                    system.gameObject.SetActive(active);
                }
            }
        }
    }
}