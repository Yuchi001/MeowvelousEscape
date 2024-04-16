using System.Collections.Generic;
using System.Linq;
using CatPackage;
using Managers.Interfaces;
using UnityEngine;
using World_Generation;

namespace Managers
{
    public delegate void CatDamageEventHandler (CatMember cat, int damage); 

    public class PlayerManager : MonoBehaviour, IManagedSingleton
    {
        
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private List<KeyColors> keyColors;
        
        [SerializeField] private GameObject catFollowerPrefab;
        [SerializeField] private GameObject catLeaderPrefab;

        public IEnumerable<KeyColors> KeyColors => keyColors;
        
        public int RescuedCatsCount { get; private set; }
        public List<ActiveCatData> BackupMembers { get; } = new();

        public List<(CatMember member, KeyCode attackKey)> TeamMembers { get; } = new();

        private (FollowLeader leaderScript, SOCat cat) _currentLeader;
        
        public static PlayerManager Instance { get; private set; }
        
        public CatDamageEventHandler OnCatDamaged;
        
        public void Init()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        private void LateUpdate()
        {
            if (_currentLeader.leaderScript == null) return;

            Camera.main.transform.position = _currentLeader.leaderScript.transform.position;
        }

        public void StartRun(SOCat cat, Vector2 spawnPos)
        {
            var catLeaderPrefab = Instantiate(this.catLeaderPrefab, spawnPos, Quaternion.identity);
            _currentLeader = (catLeaderPrefab.GetComponent<FollowLeader>(), cat);
            catLeaderPrefab.GetComponentInChildren<SpriteRenderer>().color = cat.GetDisplayInfo().CatColor;
            var member = _currentLeader.leaderScript.GetComponentInChildren<CatMember>();
            member.SetCat(new ActiveCatData()
            {
                cat = cat,
                level = 1,
                health = cat.GetSpecificInfo(1).MaxHealth,
            }, KeyCode.Q);
            TeamMembers.Add((member, KeyCode.Q));
            member.OnCatDamaged += Member_OnCatDamaged;
            worldGenerator.Generate();
        }

        private void Member_OnCatDamaged(CatMember cat, int damage)
        {
            OnCatDamaged?.Invoke(cat, damage);
        }

        private KeyCode GetAttackKey()
        {
            var keys = new List<KeyCode>()
            {
                KeyCode.Q,
                KeyCode.W,
                KeyCode.E,
                KeyCode.R,
            };
            
            TeamMembers.ForEach(m =>
            {
                keys.Remove(m.attackKey);
            });
            return keys[0];
        }
        
        public bool PickUpCat(SOCat cat, Vector2 position)
        {
            RescuedCatsCount++;
            
            var catDetails = cat.GetDisplayInfo();
            var catTeamMemberCheck = TeamMembers.FirstOrDefault(c => c.member.GetCat().cat.GetDisplayInfo().CatName == catDetails.CatName);
            if (catTeamMemberCheck != default)
            {
                catTeamMemberCheck.member.LevelUp();
                return false;
            }

            if (TeamMembers.Count < 4)
            {
                var catMember = SpawnCatFollower(cat, position);
                TeamMembers.Add((catMember, GetAttackKey()));
                catMember.OnCatDamaged += Member_OnCatDamaged;

                var catFollower = catMember.GetComponentInParent<CatFollower>();
                _currentLeader.leaderScript.AddFollower(catFollower);
                return true;
            }

            var catBackupMemberCheck =
                BackupMembers.FirstOrDefault(c => c.cat.GetDisplayInfo().CatName == catDetails.CatName);
            if (catBackupMemberCheck != default)
            {
                catBackupMemberCheck.level++;
                return false;
            }

            if (BackupMembers.Count >= 16) return false;
            
            BackupMembers.Add(new ActiveCatData()
            {
                cat = cat,
                level = 1,
                health = cat.GetSpecificInfo(1).MaxHealth,
            });
            return false;
        }

        public CatMember SpawnCatFollower(SOCat cat, Vector2 position)
        {
            var followerObject = Instantiate(catFollowerPrefab, position, Quaternion.identity);
            var memberScript = followerObject.GetComponentInChildren<CatMember>();
            memberScript.SetCat(new ActiveCatData()
            {
                cat = cat,
                level = 1,
                health = cat.GetSpecificInfo(1).MaxHealth,
            }, GetAttackKey());

            return memberScript;
        }
    }

    [System.Serializable]
    public struct KeyColors
    {
        public KeyCode key;
        public Color color;
    }
}