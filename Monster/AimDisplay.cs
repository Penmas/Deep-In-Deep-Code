using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AimDisplay : MonoBehaviour
{
    public abstract void ThrowAimDispaly(int LineNumber, Vector2 ThrowAimVector2);

    public abstract void ThrowAimUnDispaly();

}
