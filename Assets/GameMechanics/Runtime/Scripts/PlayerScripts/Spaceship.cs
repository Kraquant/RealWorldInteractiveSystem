using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Spaceship : SpaceObject, ITurnBasedObject
{
    public int TurnPriority { get => 10; set => throw new System.NotImplementedException(); }
    public SpaceObject.Action NextAction { get; set; }

    private bool _spaceshipMoving;
    private float _cellSize;
    private float _speed;


    public async Task<bool> PlayTurnAsync(TurnManager turnManager)
    {
        //Check for possible movement
        turnManager.CheckCollision(PreviewNextCoordinate(NextAction).Item1, out TurnManager.CollisionType collision, out SpaceObject collided);
        if (collision == TurnManager.CollisionType.Object && collided == this) collision = TurnManager.CollisionType.None;

        switch (collision)
        {
            case TurnManager.CollisionType.None:

                MoveCoordinate(NextAction);
                await UpdateSpaceObjectTransformAsync(turnManager.Terrain.CellSize, 10f); // CellSizeToDefine;
                break;
            case TurnManager.CollisionType.Terrain:
                break;
            case TurnManager.CollisionType.Object:
                throw new System.NotImplementedException();
            default:
                break;
        }

        return true;
    }

    public async Task<bool> UpdateSpaceObjectTransformAsync(float cellSize, float speed)
    {
        _cellSize = cellSize;
        _speed = speed;

        Vector3 lookToVec = HexCoordinates.direction_vectors[(int)ObjectOrientation].GetVector3Position();
        transform.right = lookToVec;

        CancellationTokenSource cts = new CancellationTokenSource();
        _spaceshipMoving = true;

        await WaitUntilAsync(() => _spaceshipMoving == false, 100, cts.Token);

        return true;
    }

    private void Update()
    {
        if (_spaceshipMoving)
        {
            Vector3 targetPos = 2 * _cellSize * Center.GetVector3Position();
            if (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
            }
            else _spaceshipMoving = false;
        }
    }

    public async Task WaitUntilAsync(Func<bool> cond, int checkPeriod, CancellationToken token)
    {
        while (true)
        {
            if (cond() || token.IsCancellationRequested)
            {
                break;
            }
            await Task.Delay(checkPeriod, token);
        }
    }
}
