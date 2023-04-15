using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    public class GrapableObject : MonoBehaviour, IDynamicVisibility
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private MeshRenderer _meshRenderer;
        public bool IsHidden { private set; get; }

        public void Show()
        {
            _meshRenderer.enabled = true;
            _collider.enabled = true;
            IsHidden = false;
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
            _collider.enabled = false;
            IsHidden = true;
        }
    }
}
