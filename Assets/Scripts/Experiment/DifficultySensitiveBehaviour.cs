using System.Collections.Generic;
using PsychImmersion.Experiment;
using UnityEngine;

namespace PsychImmersion
{
    /// <summary>
    /// Anything that needs to have its behaivor change depending on the difficulty should extend this class
    /// </summary>
    public abstract class DifficultySensitiveBehaviour : MonoBehaviour
    {
        public static IEnumerable<DifficultySensitiveBehaviour> Behaviours
        {
            get { return RegisteredBehaviours.Values; }
        }

        private static readonly Dictionary<int, DifficultySensitiveBehaviour> RegisteredBehaviours = new Dictionary<int, DifficultySensitiveBehaviour>();

        public static void SetLevelForAll(Difficulity level)
        {
            foreach(var b in Behaviours) b.SetLevel(level);
        }

        public virtual void Awake()
        {
            RegisteredBehaviours[GetInstanceID()] = this;
        }

        private void OnDestroy()
        {
            RegisteredBehaviours.Remove(GetInstanceID());
        }

        public abstract void SetLevel(Difficulity level);
    }
}
