namespace Krakjam
{
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private PlayerController _Player;
        [SerializeField] private PlatformContainer _AvailablePlatforms;

        [SerializeField] private int _Concurrent = 4;

        private void Awake()
        {
            _InstantiatedPlatforms = new List<Platform>();
            GenerateDefault();
        }

        private List<Platform> _InstantiatedPlatforms;

        [Button]
        private void GenerateDefault()
        {
            var defaultPrefabs = _AvailablePlatforms.Default;
            foreach (var prefab in defaultPrefabs)
            {
                var position = Vector3.zero;
                if (_InstantiatedPlatforms.Count > 0)
                {
                    position = _InstantiatedPlatforms.Last().EndPoint;
                }

                var platform = Instantiate(prefab, transform);
                platform.Finished += OnPlatformFinished;
                platform.transform.position = position;

                _InstantiatedPlatforms.Add(platform);
            }

            while (_InstantiatedPlatforms.Count < _Concurrent)
            {
                GenerateNext();
            }
        }

        [Button]
        private void GenerateNext()
        {
            var prefab = _AvailablePlatforms.Draw();

            var position = Vector3.zero;
            if (_InstantiatedPlatforms.Count > 0)
            {
                position = _InstantiatedPlatforms.Last().EndPoint;
            }

            var platform = Instantiate(prefab, transform);
            platform.Finished += OnPlatformFinished;
            platform.transform.position = position;

            _InstantiatedPlatforms.Add(platform);
        }

        private void DestroyLast()
        {
            while (_InstantiatedPlatforms.Count > _Concurrent * 2)
            {
                Destroy(_InstantiatedPlatforms[0].gameObject);
                _InstantiatedPlatforms.RemoveAt(0);
            }
        }

        private void OnPlatformFinished()
        {
            GenerateNext();
            DestroyLast();
        }
    }
}