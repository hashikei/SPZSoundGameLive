using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public struct Param
    {
        public Line line { get; private set; }
        public float time { get; private set; }

        public Param (Line line, float time) {
            this.line = line;
            this.time = time;
        }
    }

    public Param param { get; private set; }

    public void Initialize(Param param) {
        this.param = param;
    }
}
