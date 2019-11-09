using System.Collections;
using UnityEngine;

namespace ClumsyBat.Players
{
    public class LevelAnimationSequencer
    {
        private Player player;
        private Rigidbody2D playerBody;

        public enum Sequences
        {
            CaveEntrance, CaveExit
        }

        private enum States
        {
            None, SettingUp, Playing, TearingDown
        }
        
        private Sequences currentSequence;
        private States state;

        private float timer;
        private Vector2 startPos;
        private Vector2 targetPos;

        public LevelAnimationSequencer(Player player)
        {
            this.player = player;
            playerBody = player.model.GetComponent<Rigidbody2D>();
            state = States.None;
        }

        public IEnumerator PlaySequence(Sequences sequence)
        {
            state = States.SettingUp;
            currentSequence = sequence;

            while (state != States.None)
            {
                yield return null;
            }
        }

        public void Update(float deltaTime)
        {
            if (state == States.None) return;

            switch (currentSequence)
            {
                case Sequences.CaveEntrance:
                    CaveEntranceAnimation(deltaTime);
                    break;
                case Sequences.CaveExit:
                    CaveExitAnimation(deltaTime);
                    break;
                default:
                    break;
            }
        }

        private void CaveEntranceAnimation(float deltaTime)
        {
            const float duration = 1f;

            switch (state)
            {
                case States.SettingUp:
                    GameStatics.Player.PossessByAI();

                    timer = 0f;
                    targetPos = new Vector2(-3, 1.3f);
                    startPos = new Vector2(-Toolbox.TileSizeX / 2, -0.7f);

                    GameStatics.Player.SetPlayerPosition(startPos);
                    GameStatics.Player.Clumsy.FaceRight();

                    player.Physics.Disable();
                    player.Abilities.Perch.Unperch();
                    player.Animate(ClumsyAnimator.ClumsyAnimations.Hover);
                    player.fog.StartOfLevel();

                    state = States.Playing;
                    break;
                case States.Playing:
                    if (timer > duration)
                    {
                        state = States.TearingDown;
                        break;
                    }

                    timer += deltaTime;
                    float animRatio = timer / duration;
                    var pos = player.model.position;
                    pos.x = startPos.x - (startPos.x - targetPos.x) * animRatio;
                    pos.y = startPos.y - (startPos.y - targetPos.y) * Mathf.Pow(animRatio, 2);
                    playerBody.velocity = (pos - player.model.position).normalized * player.moveSpeed;
                    break;
                case States.TearingDown:
                    player.Animate(ClumsyAnimator.ClumsyAnimations.FlapSlower);
                    player.Physics.Enable();
                    player.Physics.SetVelocity(6f, 8f);
                    state = States.None;
                    break;
            }
        }

        private void CaveExitAnimation(float deltaTime)
        {
            const float duration = 0.9f;

            switch (state)
            {
                case States.SettingUp:
                    GameStatics.Player.PossessByAI();

                    player.Physics.Disable();
                    player.Abilities.Perch.Unperch();

                    timer = 0f;
                    startPos = player.model.position;
                    targetPos = new Vector3(player.model.position.x + Toolbox.TileSizeX / 2f, -0.5f, player.model.position.z);

                    state = States.Playing;
                    break;
                case States.Playing:
                    if (timer > duration)
                    {
                        state = States.TearingDown;
                        break;
                    }

                    timer += deltaTime;
                    player.model.position = Vector3.Lerp(startPos, targetPos, timer / duration);
                    break;
                case States.TearingDown:
                    player.lantern.transform.position += new Vector3(.3f, 0f, 0f);
                    player.fog.EndOfLevel();
                    player.Physics.Body.velocity = Vector2.zero;
                    state = States.None;
                    break;
            }
        }
    }
}
