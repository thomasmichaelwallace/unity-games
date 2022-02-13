using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerBehaviour : MonoBehaviour
{
    private const float Speed = ScreenConfiguration.TrackVerticalDistance / 0.5f; // half a second to switch
    private const float CoolingTime = 0.10f;
    private static readonly int IsSwitching = Animator.StringToHash("isSwitching");
    private static readonly int ToDead = Animator.StringToHash("toDead");
    [SerializeField] private TrackBehaviour track;
    [SerializeField] private GameBehaviour game;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource dieSound;
    private Animator _animator;
    private float _remainingCoolingTime;
    private bool _cooling;
    private bool _dying;
    private float _dyingFrame = -0.25f;
    private Vector3 _moveTarget;
    private bool _moving;
    private int _row = 1;
    private bool _wasFalling;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // start cooling to prevent game-over leaking across
        _cooling = true;
        _remainingCoolingTime = 0.5f; // half a second of safety 
    }

    private void Update()
    {
        var p = transform.position;

        // cannot control while moving
        if (_moving)
        {
            transform.position = Vector3.MoveTowards(p, _moveTarget, Speed * Time.deltaTime);
            if (transform.position != _moveTarget) return;
            _moving = false;
            _cooling = true;
            _animator.SetBool(IsSwitching, false);
            return;
        }

        if (_dying)
        {
            _dyingFrame += Time.deltaTime;
            if (_dyingFrame < 0) return; // holding frame
            var t = transform;
            if (_dyingFrame > 1)
            {
                // completed
                t.localScale = Vector3.zero;
                return;
            }

            // animating
            t.position += new Vector3(Time.deltaTime, -Time.deltaTime);
            t.localScale = new Vector3(1 - _dyingFrame, 1 - _dyingFrame);
            return;
        }
        
        // cool off between key presses
        if (_cooling)
        {
            _remainingCoolingTime -= Time.deltaTime;
            if (_remainingCoolingTime > 0) return;
            _cooling = false;
            _remainingCoolingTime = CoolingTime;
        }

        if (!IsSafe())
        {
            if (game.FallIsDead())
            {
                _dying = true;
                _animator.SetBool(IsSwitching, false);
                _animator.SetTrigger(ToDead);
                dieSound.Play();
            }
            else
            {
                _animator.SetBool(IsSwitching, true);
                _wasFalling = true;
            }

            return;
        }

        if (_wasFalling)
        {
            _wasFalling = false;
            _animator.SetBool(IsSwitching, false);
        }
        

        // up/down to move
        var vertical = Input.GetAxis("Vertical");
        if (vertical != 0)
        {
            if (vertical > 0 && _row <= 0) return; // stop going up
            if (vertical < 0 && _row >= ScreenConfiguration.YCount - 1) return; // stop going down

            _moving = true;
            jumpSound.Play();
            var d = new Vector3(-ScreenConfiguration.XOffset, ScreenConfiguration.TrackVerticalDistance);
            _moveTarget = p + Mathf.Sign(vertical) * d;
            _row += vertical < 0 ? 1 : -1;
            _animator.SetBool(IsSwitching, true);
        }
    }

    private bool IsSafe()
    {
        var p = transform.position;

        for (float o = 1; o < 8; o += 1)
        {
            var px = o / ScreenConfiguration.Ppu;
            var at = new Vector3(p.x + px, p.y - ScreenConfiguration.Unit);
            if (track.GetAtPosition(at) != LevelBehaviour.Tracks.Empty) return true;
        }

        return false;
    }
}