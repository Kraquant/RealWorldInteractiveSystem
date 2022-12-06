using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class SpaceUtilities
{
    static public async Task WaitUntilAsync(Func<bool> cond, int checkPeriod, CancellationToken token)
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
