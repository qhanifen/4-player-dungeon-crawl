using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration
{
    [ExecuteInEditMode]
    public class Room : MonoBehaviour
    {
        public Junction[] junctions;
        public int JunctionCount { get { return junctions.Length; } }
                
        public void CreateJunction()
        {
            Instantiate(new Junction(), this.transform.position, Quaternion.identity, this.transform);
        }

        public void RotateRoom(Junction joinedJunction)
        {
            //Get a random junction from the room
            Junction junction = junctions[Random.Range(0, junctions.Length)];
            Vector3 tangentVector = Vector3.Scale(joinedJunction.transform.position, Vector3.back);


        }
    }
}