using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 分配器UI
/// </summary>
[RequireComponent(typeof(Bubble))]
public class DistributorUI : MonoBehaviour
{
    [HideInInspector]
    [NonSerialized]
    public Distributor distributor;

    public DistributorPortStateButton stateButtonLeft;
    public DistributorPortStateButton stateButtonRight;
    public DistributorPortStateButton stateButtonUp;
    public DistributorPortStateButton stateButtonDown;

    private void Start()
    {
        if (distributor == null)
            return;

        stateButtonLeft.PortType = distributor.GetPortType(Vector2Int.left);
        stateButtonRight.PortType = distributor.GetPortType(Vector2Int.right);
        stateButtonUp.PortType = distributor.GetPortType(Vector2Int.up);
        stateButtonDown.PortType = distributor.GetPortType(Vector2Int.down);
    }
}
