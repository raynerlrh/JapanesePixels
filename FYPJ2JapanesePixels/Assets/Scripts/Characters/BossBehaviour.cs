using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss
{
    public struct BossSpecial
    {
        public float numMinions;
        public bool summonMinions;
    }

    public class BossBehaviour : DefaultCharacter
    {
        BossSpecial special;
        TimerRoutine dehighlightTimer;
        TimerRoutine dehighlightTimer2;
        TimerRoutine bossCooldown;

        public enum BattleState
        {
            E_NOTDECIDED,
            E_LIGHTATTACK,
            E_MEDIUMATTACK,
            E_HEAVYATTACK
        }

        public BattleState e_battleState;

        public override void InitChar(float maxhealthval = 100)
        {
            base.InitChar(maxhealthval);
            special.numMinions = 4;
            special.summonMinions = false;
            dehighlightTimer = EnemyMoveController.instance.gameObject.AddComponent<TimerRoutine>();
            dehighlightTimer.initTimer(2);
            dehighlightTimer.executedFunction = new TimerRoutine.Delegated(dehighlightAttackGrid);
            dehighlightTimer2 = EnemyMoveController.instance.gameObject.AddComponent<TimerRoutine>();
            dehighlightTimer2.initTimer(3);
            dehighlightTimer2.executedFunction = dehighlight;
            bossCooldown = EnemyMoveController.instance.gameObject.AddComponent<TimerRoutine>();
            bossCooldown.initTimer(2);
            bossCooldown.executedFunction = unPause;
            e_battleState = BattleState.E_NOTDECIDED;
        }

        public void highlightAttackGrid()
        {
            for (int i = 0; i < special.numMinions; ++i)
            {
                Vector3Int cellpos = GameModeManager.instance.gameGrid.GetTopRightCell();
                cellpos = new Vector3Int(cellpos.x, cellpos.y - i * 2, cellpos.z);
                for (int j = cellpos.x; j >= cellpos.x - 7; --j)
                {
                    Vector3Int highlightCell = new Vector3Int(j, cellpos.y, cellpos.z);
                    TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_ENEMY, highlightCell, TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_WARNING));
                }
            }
        }

        public void highlightAttackGrid(Vector3Int[] cells)
        {
            for (int i = 0; i < cells.Length; ++i)
            {
                TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_ENEMY, cells[i], TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_WARNING));
            }
        }

        public void dehighlightAttackGrid()
        {
            for (int i = 0; i < special.numMinions; ++i)
            {
                Vector3Int cellpos = GameModeManager.instance.gameGrid.GetTopRightCell();
                cellpos = new Vector3Int(cellpos.x, cellpos.y - i * 2, cellpos.z);
                for (int j = cellpos.x; j >= cellpos.x - 7; --j)
                {
                    Vector3Int highlightCell = new Vector3Int(j, cellpos.y, cellpos.z);
                    TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_ENEMY, highlightCell, null);
                }
            }
        }

        public void dehighlight()
        {
            Vector3Int[] cellArr = GameModeManager.instance.gameGrid.GetPerimeter(2);
            TileRefManager.instance.EraseTiles(cellArr);
        }

        public void unPause()
        {
            EnemyMoveController.instance.GetComponent<RLEnvironment>().envVars.pauseAction = false;
            EnemyMoveController.instance.GetComponent<RLEnvironment>().StopCoroutine(EnemyMoveController.instance.GetComponent<RLEnvironment>().mainRoutine);
            EnemyMoveController.instance.GetComponent<RLEnvironment>().mainRoutine = EnemyMoveController.instance.StartCoroutine(EnemyMoveController.instance.GetComponent<RLEnvironment>().Act());
        }

        public void lightAttack()
        {
            if (special.summonMinions)
            {
                for (int i = 0; i < special.numMinions; ++i)
                {
                    Vector3Int cellpos = GameModeManager.instance.gameGrid.GetTopRightCell();
                    cellpos = new Vector3Int(cellpos.x, cellpos.y - i * 2, cellpos.z);
                    Vector3 temp = GameModeManager.instance.gameGrid.GetCellWPOS(cellpos);
                    GameObject summon = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[0], EnemyMoveController.instance.GetBossObj.transform, false);
                    summon.transform.position = temp;
                    dehighlightTimer.executeFunction();
                    summon.SetActive(true);
                }
                special.summonMinions = false;
                //EnemyMoveController.instance.GetComponent<RLEnvironment>().pauseAction = true;
            }
        }

        public void mediumAttack()
        {
            GameGrid grid = GameModeManager.instance.gameGrid;
            Vector3Int[] cells = GameModeManager.instance.gameGrid.GetPerimeter(2);
            for (int i = 0; i < cells.Length; ++i)
            {
                GameObject summon = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[1], EnemyMoveController.instance.GetBossObj.transform, false);
                summon.transform.position = grid.GetCellMiddleWPOS(new Vector3Int(cells[i].x, grid.GetTopRightCell().y + cells[i].y, cells[i].z));
                summon.SetActive(true);
                summon.GetComponent<Minions>().cellDes = cells[i];
            }
        }

        public void heavyAttack(Vector3Int initialPos, Vector3Int targetPos)
        {
            GameObject summon = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[1], EnemyMoveController.instance.GetBossObj.transform, false);
            summon.SetActive(true);
            summon.GetComponent<Minions>().m_MinionType = Minions.MinionType.E_PROJECTILE;
        }

        /// <summary>
        /// makes enemy move forward
        /// </summary>
        /// <param name="forwardvec">facing vector</param>
        public void moveForward(Vector3Int forwardvec, int lockAxis = 0)
        {
            GameGrid grid = GameModeManager.instance.gameGrid;
            Vector3Int playercell = PlayerMoveController.instance.GetPlayerCellPos;
            Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(EnemyMoveController.instance.GetBossObj.transform.position);
            int cellsAway = grid.GetCellsAwaySquared(mycell, playercell);
            if (lockAxis == 0 || lockAxis == 1)
                forwardvec = new Vector3Int(mycell.x + 1, mycell.y, mycell.z);
            else if (lockAxis == 2)
                forwardvec = new Vector3Int(mycell.x, mycell.y + 1, mycell.z);
            for (int i = 0; i < 4; ++i)
            {
                if (grid.GetCellsAwaySquared(forwardvec, playercell) < cellsAway)
                {
                    EnemyMoveController.instance.GetBossObj.transform.position = grid.GetCellMiddleWPOS(forwardvec);
                    break;
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            if (lockAxis == 0 || lockAxis == 1)
                                forwardvec = new Vector3Int(mycell.x - 1, mycell.y, mycell.z);
                            break;
                        case 1:
                            if (lockAxis == 0 || lockAxis == 2)
                                forwardvec = new Vector3Int(mycell.x, mycell.y + 1, mycell.z);
                            break;
                        case 2:
                            if (lockAxis == 0 || lockAxis == 2)
                            {
                                forwardvec = new Vector3Int(mycell.x, mycell.y - 1, mycell.z);
                            }
                            break;
                    }
                }
            }
            return;
        }

        public bool detectPlayerApprox(int attkradius = 1)
        {
            Vector3Int playercell = PlayerMoveController.instance.GetPlayerCellPos;
            Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(EnemyMoveController.instance.GetBossObj.transform.position);
            Vector3Int cellsAway = playercell - mycell;
            if (cellsAway.magnitude <= attkradius)
                return true;
            return false;
        }

        public bool detectPlayer(int attkradius = 1)
        {
            Vector3Int[] cells = GameModeManager.instance.gameGrid.GetPerimeter(2);
            Vector3Int playercell = PlayerMoveController.instance.GetPlayerCellPos;
            for (int i = 0; i < cells.Length; ++i)
            {
                if (cells[i].Equals(playercell))
                {
                    return true;
                }
            }
            return false;
        }

        public void doAttack()
        {
            if (e_battleState == BattleState.E_NOTDECIDED)
            {
                decideAttack();
            }
            else if (e_battleState == BattleState.E_LIGHTATTACK)
            {
                EnemyMoveController.instance.currentBoss.special.summonMinions = true;
                EnemyMoveController.instance.currentBoss.lightAttack();
                e_battleState = BattleState.E_NOTDECIDED;
            }
            else if (e_battleState == BattleState.E_MEDIUMATTACK)
            {
                if (detectPlayerApprox(3))// EnemyMoveController.instance.currentBoss.detectPlayer(2))
                {
                    highlightAttackGrid(GameModeManager.instance.gameGrid.GetPerimeter(2));
                    mediumAttack();
                    dehighlightTimer2.executeFunction();
                    e_battleState = BattleState.E_NOTDECIDED;
                }
                else
                {
                    //move to player using enemy move controller
                    Vector3Int vec = new Vector3Int();
                    moveForward(vec);
                }
            }
            else if (e_battleState == BattleState.E_HEAVYATTACK)
            {
                Vector3Int pos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(EnemyMoveController.instance.GetBossObj.transform.position);
                Vector3Int targetCell = PlayerMoveController.instance.GetPlayerCellPos;
                Vector3 target = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(targetCell);
                if (pos.y == targetCell.y)
                {
                    heavyAttack(pos, targetCell);
                    e_battleState = BattleState.E_NOTDECIDED;
                }
                else
                {
                    moveForward(new Vector3Int(), 2);
                    //e_battleState = BattleState.E_NOTDECIDED;
                    //return;
                }
            }
            EnemyMoveController.instance.GetComponent<RLEnvironment>().envVars.pauseAction = true;
            bossCooldown.executeFunction();
        }

        public bool decideAttack()
        {
            int r = Random.Range(-10, 10);
            if (r <= -5)
            {
                // if distance check fails // return false
                // if not, highlight grid then attack on next
                e_battleState = BattleState.E_MEDIUMATTACK;
                return true;
            }
            else if (r > -5 && r < 5)
            {
                highlightAttackGrid();
                e_battleState = BattleState.E_LIGHTATTACK;

                return true;
            }
            else
            {
                e_battleState = BattleState.E_HEAVYATTACK;
                return true;
            }
        }
    }

}
