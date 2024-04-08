using System.Collections.Generic;
using System.Linq;
using CatPackage;
using UnityEngine;

namespace Managers
{
    public delegate void CatDamageEventHandler (CatMember cat, int damage); 

    public class PlayerManager : MonoBehaviour
    {
        public CatDamageEventHandler OnCatDamaged;
        
        [SerializeField] private GameObject catFollower;
        [SerializeField] private GameObject catLeader;
        [SerializeField] private GameObject LevelUpCanvas;
        [SerializeField] private GameObject NewCatCanvas;
        [SerializeField] private List<KeyColors> keyColors;

        [SerializeField]
        private WorldGenerator worldGenerator;

        public List<KeyColors> KeyColors => keyColors;
        
        public int RescuedCatsCount { get; private set; }
        public List<ActiveCatData> BackupMembers => backupMembers;
        public List<(CatMember member, KeyCode attackKey)> TeamMembers => teamMembers;
        public static PlayerManager Instance { get; private set; }

        private List<(CatMember member, KeyCode attackKey)> teamMembers = new();
        private List<ActiveCatData> backupMembers = new();

        private (FollowLeader leaderScript, SOCat cat) currentLeader;
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        private void LateUpdate()
        {
            if (currentLeader.leaderScript == null) return;

            Camera.main.transform.position = currentLeader.leaderScript.transform.position;
        }

        public void StartRun(SOCat cat)
        {
            var catLeaderPrefab = Instantiate(catLeader, Vector2.zero, Quaternion.identity);
            currentLeader = (catLeaderPrefab.GetComponent<FollowLeader>(), cat);
            var member = currentLeader.leaderScript.GetComponentInChildren<CatMember>();
            member.SetCat(new ActiveCatData()
            {
                cat = cat,
                level = 1,
                health = cat.GetSpecificInfo(1).maxHealth,
            }, KeyCode.Q);
            teamMembers.Add((member, KeyCode.Q));
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
            
            teamMembers.ForEach(m =>
            {
                keys.Remove(m.attackKey);
            });
            return keys[0];
        }
        
        public bool PickUpCat(SOCat cat, Vector2 position)
        {
            RescuedCatsCount++;
            
            var catDetails = cat.GetDisplayInfo();
            var catTeamMemberCheck = teamMembers.FirstOrDefault(c => c.member.GetCat().cat.GetDisplayInfo().catName == catDetails.catName);
            if (catTeamMemberCheck != default)
            {
                catTeamMemberCheck.member.LevelUp();
                Instantiate(LevelUpCanvas, new Vector3(catTeamMemberCheck.member.transform.position.x, catTeamMemberCheck.member.transform.position.y + 50, 0), Quaternion.identity);

                return false;
            }

            if (teamMembers.Count < 4)
            {
                var catMember = SpawnCatFollower(cat, position);
                teamMembers.Add((catMember, GetAttackKey()));
                catMember.OnCatDamaged += Member_OnCatDamaged;

                var catFollower = catMember.GetComponentInParent<CatFollower>();
                currentLeader.leaderScript.AddFollower(catFollower);
                return true;
            }

            var catBackupMemberCheck =
                backupMembers.FirstOrDefault(c => c.cat.GetDisplayInfo().catName == catDetails.catName);
            if (catBackupMemberCheck != default)
            {
                catBackupMemberCheck.level++;
                return false;
            }

            if (backupMembers.Count >= 16) return false;
            
            backupMembers.Add(new ActiveCatData()
            {
                cat = cat,
                level = 1,
                health = cat.GetSpecificInfo(1).maxHealth,
            });
            return false;
        }

        public CatMember SpawnCatFollower(SOCat cat, Vector2 position)
        {
            var followerObject = Instantiate(catFollower, position, Quaternion.identity);
            var memberScript = followerObject.GetComponentInChildren<CatMember>();
            memberScript.SetCat(new ActiveCatData()
            {
                cat = cat,
                level = 1,
                health = cat.GetSpecificInfo(1).maxHealth,
            }, GetAttackKey());
            Instantiate(NewCatCanvas, new Vector3(followerObject.transform.position.x, followerObject.transform.position.y + 50, 0), Quaternion.identity);

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