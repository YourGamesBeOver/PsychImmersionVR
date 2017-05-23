using System.Collections;
using UnityEngine;

namespace PsychImmersion.AI
{

    /// <summary>
    /// This script is used to expand the logical walls that the mouse and spider AI rely on for pathing when the box is gone.  
    /// See BoxManager.cs for more details.  
    /// </summary>
    public class WallExpander : MonoBehaviour
    {

        public Vector3 FinalScale = Vector3.one;
        private Vector3 _initialScale;
        public float ScaleTime = 1f;


        private void Start()
        {
            _initialScale = transform.localScale;
        }
        
        public void ExpandWalls()
        {
            StartCoroutine(ScaleCoroutine());
        }

        private IEnumerator ScaleCoroutine()
        {
            var curTime = 0f;
            while (curTime < ScaleTime)
            {
                transform.localScale = Vector3.Lerp(_initialScale, FinalScale, curTime / ScaleTime);
                yield return null;
                curTime += Time.deltaTime;
            }
            transform.localScale = FinalScale;
            this.enabled = false;
        }
    }
}
