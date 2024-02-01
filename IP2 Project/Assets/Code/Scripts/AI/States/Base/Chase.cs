using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;


namespace States.Base
{
    [System.Serializable]
    public class Chase : State
    {
        public override string Name { get => "Chasing"; }


        // These variable should never be edited outside of the 'InitialiseValues' function.
        private Func<Vector2> _target;


        // Movement Params.
        [SerializeField] private float _chaseSpeed;


        // Initialise the Chase State's values.
        //  These values would ordinarily be readonly and set via constructor, but due to the method of creating the states we are testing to allow for runtime editing of parameters, we cannot do that.
        public void InitialiseValues(Func<Vector2> target)
        {
            this._target = target;
        }
    }
}