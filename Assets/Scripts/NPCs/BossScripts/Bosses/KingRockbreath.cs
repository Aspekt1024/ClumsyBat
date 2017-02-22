using UnityEngine;
using System.Collections;

public class KingRockbreath : Boss
{
    private ParabolicProjectile _projectile;
    private JumpPound _jumpPound;
    private Walk _walk;
    private SpawnStalactites _stalControl;
    private BossMoths _mothControl;
    private bool _bSatalactitesSpawned;
    
    private void Start()
    {
        _health = 4;
        LoadAbilities();
        StartCoroutine("BossActionSequencer");
    }

    private void Update()
    {
        if (_player == null) return;
        if (!_player.IsAlive() || _state == BossStates.Dead)
        {
            StopCoroutine("BossActionSequencer");
            return;
        }
        if ((transform.position.x < _player.transform.position.x && transform.localScale.x > 0)
            || (transform.position.x > _player.transform.position.x && transform.localScale.x < 0))
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            _player.FaceOtherDirection();
        }
    }
    
    private void LoadAbilities()
    {
        _projectile = new ParabolicProjectile(transform);
        _jumpPound = gameObject.AddComponent<JumpPound>();
        _walk = gameObject.AddComponent<Walk>();
        _stalControl = gameObject.AddComponent<SpawnStalactites>();
        _mothControl = new GameObject("SceneSpawnables").AddComponent<BossMoths>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Stalactite" && _health == 1)
            TakeDamage();
    }

    public override void HitByHypersonic()
    {
        if (_health > 1)
            TakeDamage();
    }

    private IEnumerator BossActionSequencer()
    {
        yield return StartCoroutine("WaitSeconds", 2f);
        while (true)
        {
            yield return StartCoroutine("Jump");
            _projectile.ActivateProjectile(_player.transform.position);

            _walk.Activate();
            yield return StartCoroutine("WaitSeconds", 3f);
        }
    }

    private IEnumerator Jump()
    {
        _jumpPound.Activate();
        yield return StartCoroutine("WaitSeconds", 2f);
    }

    protected override void PauseGame()
    {
        base.PauseGame();
        _projectile.Pause();
    }

    protected override void ResumeGame()
    {
        base.ResumeGame();
        _projectile.Resume();
    }
    
    // TODO move this to separate helper class
    private IEnumerator WaitSeconds(float secs)
    {
        float timer = 0f;
        while (timer < secs)
        {
            if (!GetComponent<Boss>().IsPaused())
                timer += Time.deltaTime;
            yield return null;
        }
    }

    private void GroundSlam()
    {
        if (_bSatalactitesSpawned)
        {
            _stalControl.Drop();
        }
        else
        {
            _stalControl.Spawn();
        }
        _bSatalactitesSpawned = !_bSatalactitesSpawned;
    }

    protected override void SetEvents()
    {
        base.SetEvents();
        BossEvents.OnJumpLanded += GroundSlam;
    }

    protected override void RemoveEvents()
    {
        base.RemoveEvents();
        BossEvents.OnJumpLanded -= GroundSlam;
    }
}
