using UnityEngine;

public class PlayerCharacter: MonoBehaviour {


    #region Class Members

    private new Rigidbody2D rigidbody;

    #endregion


    #region MonoBehaviour Override

    void Start () {
        rigidbody = this.GetComponent<Rigidbody2D> ();
    }

    #endregion


    #region Damage and Health

    #endregion
}
