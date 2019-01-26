using UnityEngine;

/// <summary>
/// Smooth Base.
/// By Javier García, 2018.
/// 
/// <para>
/// Base for smooth damp interpolators.
/// </para>
/// </summary>
public abstract class Smooth<T> {

    protected T _target;        //  Taget for the interpolation.
    protected T _current;       //  Current value of transition.
    protected T _velocity;      //  Current velocity in interpolation.

    protected float _smooth;    //  Smoothness.

    /// <summary> Gets the velocity. </summary>
    public T velocity {
        get { return _velocity; }
    }

    /// <summary> Gets or sets the target. </summary>
    public T target {
        get { return _target; }
        set { _target = value; }
    }

    /// <summary> Gets or sets the current value. </summary>
    public T current {
        get { return _current; }
        set { _current = value; }
    }

    /// <summary> Gets the incoming value. </summary>
    public abstract T incoming { get; }

    /// <summary> Initializes a new instance of Smooth class. </summary>
    protected Smooth (float smooth) {
        _smooth = smooth;
    }

    /// <summary> Updates the position and target. </summary>
    public T Update (T current, T target) {
        this.target = target;
        return Update (current);
    }

    /// <summary> Updates the position. </summary>
    public T Update (T current) {
        this.current = current;
        return this.incoming;
    }

}


public class SmoothCapsule: Smooth<float> {

    /// <summary> Gets or sets the current value. </summary>
    public override float incoming {
        get {
            _current = Mathf.SmoothDamp (
                current: _current,
                target: _target,
                currentVelocity: ref _velocity,
                smoothTime: _smooth,
                maxSpeed: float.MaxValue,
                deltaTime: Time.deltaTime
            );
            return _current;
        }
    }

    /// <summary> Initializes a new instance of this class. </summary>
    public SmoothCapsule(float smooth) : base (smooth) { }
}
