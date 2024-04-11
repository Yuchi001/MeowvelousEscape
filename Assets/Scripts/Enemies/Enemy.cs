using System.Collections;
using CatPackage;
using Managers;
using NaughtyAttributes;
using UnityEngine;
using Utils;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        public enum EnemyState
        {
            Patrolling,
            Chasing,
        }

        public enum AttackType
        {
            Meelee,
            Nightstick,
            Ranged,
        }

        private static CatMember player;

        [SerializeField]
        private Rigidbody2D rigidbody2d;
        [SerializeField]
        private SpritesetAnimator animator;
        [ShowNonSerializedField]
        private Vector2 currentTarget;

        [Header("Patrolling")]
        [SerializeField]
        private RandomValue randomMovementRange;
        [SerializeField]
        private RandomValue randomMovementChangeDelay;
        [SerializeField]
        private RandomValue patrollingSpeed;
        [SerializeField]
        private RandomValue patrollingAnimationSpeedModifier;
        [ShowNonSerializedField]
        private Vector2 randomMovementCenter;

        [Header("Detection")]
        [SerializeField]
        private RandomValue playerDetectionRange;
        [SerializeField]
        private Animation exclamationAnimation;

        [Header("Chasing")]
        [SerializeField]
        private RandomValue chasingSpeed;
        [SerializeField]
        private float chasingAnimationSpeedModifier = 1;
        [SerializeField]
        private RandomValue escapeDistance;
        [ShowNonSerializedField]
        private bool isChasing;

        [Header("Attacking")]
        [SerializeField]
        private AttackType attackType;
        [SerializeField]
        private RandomValue meeleeAttackRange;
        [SerializeField]
        private RandomValue rangedAttackRange;
        [ShowNonSerializedField]
        private float attackRange;
        [SerializeField]
        private LayerMask projectilesBlockingLayers;
        [SerializeField]
        private RandomValue attackCooldown;
        [ShowNonSerializedField]
        private float attackCooldownTimer;
        [SerializeField]
        private CatMember attackAndChaseTarget; // cat member
        [SerializeField]
        private LineRenderer shootingLine;
        [SerializeField]
        private LayerMask damagedLayers;

        [ShowNonSerializedField]
        private bool isAttacking;

        private Collider2D[] detectedMeeleeColliders = new Collider2D[4];

        [Header("Damaging")]
        [SerializeField, Tag]
        private string projectileTag;
        [ShowNonSerializedField]
        private bool isBeingDamaged;
        [SerializeField]
        private RandomValue maxHealthValue;
        [ShowNonSerializedField]
        private int maxHealth;
        [ShowNonSerializedField]
        private int currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth = Mathf.RoundToInt(maxHealthValue.Value);
            attackType = (AttackType)Random.Range(0, 3);
        }

        private void Start()
        {
            var attackRangeRange = attackType == AttackType.Ranged ? rangedAttackRange : meeleeAttackRange;
            attackRange = attackRangeRange.Value;

            if (player == null)
                player = PlayerManager.Instance.TeamMembers[0].member;

            randomMovementCenter = transform.position;
            InvokeRepeating(nameof(ChangePatrolTarget), 0, randomMovementChangeDelay.Value);
        }

        private void ChangePatrolTarget()
        {
            currentTarget = randomMovementCenter + Random.insideUnitCircle * randomMovementRange.Value;
        }

        private void Update()
        {
            if (isAttacking)
                return;

            if (isBeingDamaged)
                return;

            var squareDistanceToPlayer = (player.transform.position - transform.position).sqrMagnitude;
            if (isChasing == false && squareDistanceToPlayer < playerDetectionRange.Value * playerDetectionRange.Value)
            {
                isChasing = true;
                attackAndChaseTarget = PlayerManager.Instance.TeamMembers[Random.Range(0, PlayerManager.Instance.TeamMembers.Count)].member;  // potem to ma być cat member

                //exclamationAnimation?.Play();
                CancelInvoke(nameof(ChangePatrolTarget));
            }
            else if (isChasing && squareDistanceToPlayer > escapeDistance.Value * escapeDistance.Value)
            {
                isChasing = false;
                InvokeRepeating(nameof(ChangePatrolTarget), 0, randomMovementChangeDelay.Value);
            }

            var speed = isChasing ? chasingSpeed.Value : patrollingSpeed.Value;
            if (isChasing)
            {
                currentTarget = attackAndChaseTarget.transform.position;
            }

            var direction = currentTarget - (Vector2)transform.position;
            if (direction.sqrMagnitude > 0.1f)
            {
                if (direction.sqrMagnitude > 1)
                    direction.Normalize();

                var animationSpeedModifier = isChasing ? chasingAnimationSpeedModifier : patrollingAnimationSpeedModifier.Value;
                var animation = OfficerAnimation.Walking;
                if (isChasing)
                {
                    switch (attackType)
                    {
                        case AttackType.Meelee:
                            animation = OfficerAnimation.Running;
                            break;
                        case AttackType.Nightstick:
                            animation = OfficerAnimation.RunningWithNightstick;
                            break;
                        case AttackType.Ranged:
                            var relativePosition = attackAndChaseTarget.transform.position - transform.position;
                            float shootingLineY = 1.225f;
                            if (Mathf.Abs(relativePosition.y) < Mathf.Abs(relativePosition.x))
                            {
                                animation = OfficerAnimation.RunningWithGunFront;
                            }
                            else if (relativePosition.y > 0)
                            {
                                shootingLineY = 1.68f;
                                animation = OfficerAnimation.RunningWithGunUp;
                            }
                            else
                            {
                                shootingLineY = 0.62f;
                                animation = OfficerAnimation.RunningWithGunDown;
                            }

                            var shootingLineX = 0.625f;
                            if (animator.SpriteRenderer.flipX)
                                shootingLineX *= -1;
                            if (shootingLine)
                                shootingLine.transform.localPosition = new Vector3(shootingLineX, shootingLineY);

                            break;
                    }
                }
                else
                {
                    animation = attackType switch
                    {
                        AttackType.Nightstick => OfficerAnimation.WalkingWithNightstick,
                        AttackType.Ranged => OfficerAnimation.WalkingWithGunFront,
                        _ => animation
                    };
                }

                SetAnimation(animation);
                animator.AnimationSpeed = animationSpeedModifier * Mathf.Max(SpritesetAnimator.idleAnimationSpeed, speed);
                var xDirection = direction.x;
            
                animator.SpriteRenderer.flipX = xDirection switch
                {
                    < 0 => true,
                    > 0 => false,
                    _ => animator.SpriteRenderer.flipX
                };

                rigidbody2d.velocity = direction * speed;
            }
            else
            {
                SetAnimation(OfficerAnimation.Standing);
                animator.AnimationSpeed = SpritesetAnimator.idleAnimationSpeed;
                rigidbody2d.velocity = Vector2.zero;
            }

            if (!isChasing) return;
            attackCooldownTimer += Time.deltaTime;
        
            if (!CanAttack()) return;
            attackCooldownTimer = 0;
            Attack();
        }

        private void SetAnimation(OfficerAnimation animation)
        {
            animator.SetAnimation((int)animation);
        }

        public bool CanAttack()
        {
            if (attackCooldownTimer < attackCooldown.Value)
                return false;

            if (attackAndChaseTarget && Vector2.Distance(attackAndChaseTarget.transform.position, transform.position) > attackRange)
                return false;

            return attackType != AttackType.Ranged || !Physics2D.Linecast(transform.position, attackAndChaseTarget.transform.position, projectilesBlockingLayers);
        }

        private void Attack()
        {
            rigidbody2d.velocity = Vector2.zero;

            isAttacking = true;
            if (attackType == AttackType.Ranged)
            {
                if (shootingLine)
                {
                    shootingLine.SetPosition(0, shootingLine.transform.position);
                    shootingLine.SetPosition(1, attackAndChaseTarget.transform.position);
                    shootingLine.enabled = true;
                    attackAndChaseTarget.TakeDamage(1);
                }
            }
            else
            {
                var animation = OfficerAnimation.AttackingWithNightstick;
                if (attackType == AttackType.Meelee)
                {
                    var randomValue = Random.value;
                    animation = randomValue > 0.75f ? OfficerAnimation.KarateKick : randomValue > 0.45f ? OfficerAnimation.Kicking : OfficerAnimation.Punching;
                }

                var count = Physics2D.OverlapCircleNonAlloc(transform.position, attackRange, detectedMeeleeColliders, damagedLayers);
                for (var i = 0; i < count; i++)
                {
                    if (detectedMeeleeColliders[i].TryGetComponent<CatMember>(out var member))
                    {
                        member.TakeDamage(1);
                    }
                }
                SetAnimation(animation);
            }
            Invoke(nameof(EndAttacking), 0.2f);
        }

        private void EndAttacking()
        {
            if (shootingLine)
                shootingLine.enabled = false;

            isAttacking = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isBeingDamaged != false || !collision.CompareTag(projectileTag)) return;
            var damage = 1;
            //if (collision.TryGetComponent<Projectile>(out var projectile))
            //    damage = projectile.Damage;
            TakeDamage(damage);
            Destroy(collision.gameObject);
        }

        private void TakeDamage(int damage)
        {
            isBeingDamaged = true;
            currentHealth -= damage;
            rigidbody2d.velocity = Vector3.zero;
            var animation = currentHealth <= 0 ? OfficerAnimation.FallingDown : Random.value < 0.5f ? OfficerAnimation.DamageStrong : OfficerAnimation.DamageWeak;
            animator.AnimationSpeed = 6;
            SetAnimation(animation);
            Invoke(nameof(StopBeingDamaged), 0.5f);
        }

        private void StopBeingDamaged()
        {
            if (currentHealth <= 0)
            {
                StartCoroutine(DisappearCo());
                return;
            }
        
            isBeingDamaged = false;
        }

        private IEnumerator DisappearCo()
        {
            rigidbody2d.velocity = Vector2.zero;
            for (float progress = 0; progress < 1; progress += Time.deltaTime)
            {
                var color = animator.SpriteRenderer.color;
                color.a = 1 - progress;
                animator.SpriteRenderer.color = color;
                yield return null;  
            }
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.DrawSphere(transform.position, attackRange);
            }
        }
    }
}