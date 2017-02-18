using UnityEngine;

public class FlapComponent {

    private Player _player;
    private const float _horizontalVelocity = 3f;
    private const float _verticalVelocity = 9f;

    public FlapComponent(Player player)
    {
        _player = player;
    }

    public enum MovementMode
    {
        VerticalOnly,
        HorizontalEnabled
    }
    public MovementMode Mode;

    public void Flap(InputManager.TapDirection tapDir = InputManager.TapDirection.Center)
    {
        float horizVelocity = GetHorizontalVelocity(tapDir);
        var velocity = new Vector2(horizVelocity, _verticalVelocity);
        _player.SetVelocity(velocity);

        GameData.Instance.Data.Stats.TotalJumps++;
        _player.SetAnimation("Flap");
        _player.PlaySound(ClumsyAudioControl.PlayerSounds.Flap);
    }

    private float GetHorizontalVelocity(InputManager.TapDirection tapDir)
    {
        float horizVelocity = 0f;
        if (Mode == MovementMode.HorizontalEnabled)
        {
            if (tapDir == InputManager.TapDirection.Left)
            {
                horizVelocity = -_horizontalVelocity;
            }
            else if (tapDir == InputManager.TapDirection.Right)
            {
                horizVelocity = _horizontalVelocity;
            }
        }
        return horizVelocity;
    }
}
