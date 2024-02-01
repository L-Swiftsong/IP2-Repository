using UnityEngine;
using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Idle : State
    {
        public override string Name { get => "Idle"; }


        [SerializeField] private float _minIdleDelay;
        [SerializeField] private float _maxIdleDelay;
        [SerializeField, ReadOnly] private float _idleDelayComplete;

        public bool IsDelayComplete { get => Time.time >= _idleDelayComplete;}


        public override void OnEnter()
        {
            base.OnEnter();

            _idleDelayComplete = Time.time + Random.Range(_minIdleDelay, _maxIdleDelay);
        }
        public override void OnExit()
        {
            base.OnExit();

            _idleDelayComplete = 0f;
        }
    }
}