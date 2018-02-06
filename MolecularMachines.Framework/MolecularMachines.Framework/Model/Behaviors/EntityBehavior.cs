using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Implementations of this class control the behavior of an <see cref="Entity"/>.
    /// For example, reading <see cref="Sensor"/> data, and induce <see cref="Conformation"/>-changes.
    /// It provides some helper methods to make this task more comfortable.
    /// Also have a look at <see cref="BindingSiteAttribute"/>, <see cref="BoundEntityAttribute"/>,
    /// <see cref="EntityBehaviorIdAttribute"/>, <see cref="MarkerAttribute"/>, <see cref="SensorAttribute"/>,
    /// <see cref="StateAttribute"/>.
    /// 
    /// The class is implemented using a state machine. Define states by public methods without parameters
    /// and apply the <see cref="StateAttribute"/> on them. Via the <see cref="SetState(StateMethod)"/>
    /// method, the current state can be changed. The current state method is called for every frame.
    /// </summary>
    public abstract class EntityBehavior : IFrameUpdate, IDisposable
    {
        public EntityBehavior()
        {

        }

        private EntityBehaviorInjector injector = null;

        private Entity owner = null;

        /// <summary>
        /// Gets or sets the entity that is controlled by this behavior.
        /// </summary>
        public Entity Owner
        {
            get { return owner; }
            set
            {
                this.owner = value;
                injector = new EntityBehaviorInjector(this, owner);
                this.Start();
            }
        }

        /// <summary>
        /// Called as soon as an <see cref="Owner"/> entity was assigned.
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// Must be called every frame.
        /// Updates bound entity properties (<see cref="BoundEntityAttribute"/>) and 
        /// executes the current state method.
        /// </summary>
        public virtual void Update()
        {
            if (injector != null)
            {
                injector.UpdateValues();
            }

            if (this.currentState != null)
            {
                this.currentState();
            }
        }

        /// <summary>
        /// Change the conformation of the controlled entity (<see cref="Owner"/>).
        /// </summary>
        /// <param name="conformationId">ID of the new conformation</param>
        /// <param name="duration">Duration in seconds</param>
        protected void SetConformation(string conformationId, float duration = 1.0f)
        {
            Owner.Structure.SetConformation(conformationId, duration);
        }

        /// <summary>
        /// Change the conformation of the controlled entity (<see cref="Owner"/>).
        /// </summary>
        /// <param name="conformation">new conformation</param>
        /// <param name="duration">Duration in seconds</param>
        protected void SetConformation(Conformation conformation, float duration = 1.0f)
        {
            Owner.Structure.SetConformation(conformation, duration);
        }

        private StateMethod currentState;
        private DateTime lastStateChange = DateTime.Now;

        /// <summary>
        /// Set the current state method.
        /// The current state method will then be called for every frame (until the current state is changed again).
        /// State methods must have the <see cref="StateAttribute"/> applied.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(StateMethod state)
        {
            if (state == null) { throw new Exception("SetState: state is null"); }

            var attributes = state.Method.GetCustomAttributes(typeof(StateAttribute), false);
            if (attributes.Length != 1)
            { throw new Exception("a State-Method must have exactly one State-Attribute. " + state.Method.Name + " has " + attributes.Length); }
            else
            {
                var stateAttribute = (StateAttribute)(attributes[0]);
                var conformation = stateAttribute.Conformation;
                if (conformation != null)
                {
                    this.SetConformation(conformation);
                }

                lastStateChange = DateTime.Now;
                OnStateChange(this.currentState, state);

                this.currentState = state;
            }
        }

        protected float SecondsInState
        {
            get { return (float)(DateTime.Now - this.lastStateChange).TotalSeconds; }
        }

        /// <summary>
        /// Is called during the transition from one state to another.
        /// </summary>
        /// <param name="oldState">The old state</param>
        /// <param name="newState">The new state</param>
        protected virtual void OnStateChange(StateMethod oldState, StateMethod newState) { }

        public bool IsCurrentState(StateMethod state)
        {
            return (StateMethod.Equals(this.currentState, state));
        }

        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// Id called when <see cref="Dispose"/> is called.
        /// </summary>
        protected virtual void OnDispose()
        {

        }
    }
}
