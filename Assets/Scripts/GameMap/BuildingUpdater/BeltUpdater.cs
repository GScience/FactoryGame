using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BeltUpdater : BeltUpdaterBase
{
    public override float Speed => 1f;

    public override void OnCarryingItem(Belt building, float process)
    {
        building.UpdateAnimation();
    }

    public override bool TryFinishItemCarrying(Belt building)
    {
        if (building.outputBuilding == null)
            return false;

        var output = building.outputBuilding;

        return output.TryPutOneItem(building.cargo);
    }

    public override void OnBuilt(Belt building)
    {
        
    }

    public override void OnCrash(Belt building)
    {
        
    }

    public override void OnStart(Belt building)
    {
        
    }

    public override void OnStop(Belt building)
    {
        
    }
}
