namespace Krakjam
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class InputController : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Submit"))
            {
                Debug.LogError("X:");
            }
        }
    }
}