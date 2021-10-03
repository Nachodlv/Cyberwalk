using System;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
   [SerializeField] private LayerMask avoidPlayerMask;
   [SerializeField] private LayerMask colliderPlayerMask;

   private float waitTime;

   private void Update()
   {
      if (Input.GetButtonUp("Vertical"))
      {
         waitTime = 0.2f;
      }

      if (Input.GetAxisRaw("Vertical") < 0)
      {
         if (waitTime <= 0)
         {
            gameObject.layer = ToLayer(avoidPlayerMask);
            waitTime = 0.2f;
         }
         else
         {
            waitTime -= Time.deltaTime;
         }
      }

      if (Input.GetAxisRaw("Vertical") > 0 || Input.GetButton("Jump"))
      {
         gameObject.layer = ToLayer(colliderPlayerMask);
      }
   }

   public static int ToLayer ( int bitmask ) {
      int result = bitmask>0 ? 0 : 31;
      while( bitmask>1 ) {
         bitmask = bitmask>>1;
         result++;
      }
      return result;
   }

}
