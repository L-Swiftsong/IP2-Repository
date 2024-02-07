using UnityEngine;
using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Dead : State
    {
        public override string Name { get => "Dead"; }


        [SerializeField] private GameObject _objectToDestroy;
        [SerializeField] private float _destroyDelay;


        public override void OnEnter()
        {
            base.OnEnter();

            GameObject.Destroy(_objectToDestroy, _destroyDelay);
        }
    }
}