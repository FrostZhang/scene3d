using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HassEntity : MonoBehaviour
{
    public string Entity_id { get => entity_id; } //homeassitant entity ID
    public Collider editC;  //交互体
    protected string entity_id;
    //const string switchkey = "switch";//对标 homeassistant 状态协议 on off
    //const string lightkey = "light";  //对标 homeassistant 状态协议 on off
    //const string climate = "climate"; //对标 homeassistant 状态协议 on off
    //const string binary_sensor = "binary_sensor"; //对标 homeassistant 状态协议 on off
    //const string media_player = "media_player"; //对标 homeassistant 状态协议 idle
    //const string person = "person"; //对标 homeassistant 状态协议 not_home
    //const string cover = "cover"; //对标 homeassistant 状态协议 closed closing open opening
    //const string fan = "fan"; //对标 homeassistant 状态协议 on off
    const string on = "on"; //对标 homeassistant 状态协议
    const string off = "off"; //对标 homeassistant 状态协议
    protected const string undefine = "undefine"; //对标 homeassistant 状态协议

    string head;    //todo 由用户提供，在面板绑定ID会被解析作为指令发给homeassitant，这里有bug！
    const string cmdon = "turn_on"; //对标 homeassistant 状态协议
    const string cmdoff = "turn_off"; //对标 homeassistant 状态协议
    protected bool open = false;    //获得状态，目前只接了homeassitant开关。原因全接数据量过大还没优化！
    string lastState;
    /// <summary> bingding homeassistant </summary>
    public void SetEntity(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            head = null;
            entity_id = null;
            return;
        }
        var ids = id.Split('.');
        if (ids.Length == 2)
        {
            head = ids[0];
            entity_id = id;
        }
    }

    /// <summary> Click From mouse or touch </summary>
    public virtual void Click()
    {
        if (string.IsNullOrEmpty(entity_id))
        {
            if (open)
            {
                open = false;
                TrunOff();
            }
            else
            {
                open = true;
                TrunOn();
            }
            return;
        }
        HassServerMessage message = new HassServerMessage();
        message.head = head;
        message.entity_id = entity_id;
        if (open)
        {
            open = false;
            message.cmd = cmdoff;
            TrunOff();
        }
        else
        {
            open = true;
            message.cmd = cmdon;
            TrunOn();
        }
        Shijie.ClickMessage(JsonUtility.ToJson(message));
    }

    /// <summary> emmm  when mouseon</summary>
    public virtual void MouseOn()
    {

    }
    protected virtual void TrunOn()
    {
        
    }
    protected virtual void TrunOff()
    {
        
    }
    /// <summary>from hass message. not on/off</summary>
    protected virtual void HassDestine(string state)
    {

    }

    /// <summary>after mouse longclick or touchlong</summary>
    protected virtual void AfterLongClick()
    {

    }

    /// <summary>from hass message open cmd</summary>
    protected virtual void HassStateOn()
    {
        
    }

    /// <summary>from hass message close cmd</summary>
    protected virtual void HassStateOff()
    {

    }

    /// <summary> from hass message ans: on/off/other</summary>
    public void StateMeassge(string state)
    {
        if (state == lastState)
            return;
        lastState = state;
        if (state == on)
        {
            if (!open)
            {
                open = true;
                HassStateOn();
                TrunOn();
            }
        }
        else if (state == off)
        {
            if (open)
            {
                open = false;
                HassStateOff();
                TrunOff();
            }
        }
        else
        {
            HassDestine(state);
        }
    }

    /// <summary>when mouse longclick or touchlong</summary>
    public virtual void LongClick()
    {
        if (!string.IsNullOrEmpty(entity_id))
        {
            HassMoreInfo info = new HassMoreInfo();
            info.entity_id = entity_id;
            Shijie.LongClickMessage(JsonUtility.ToJson(info));
        }
        AfterLongClick();
    }

    public virtual void MouseExit()
    {

    }

    /// <summary>into editor or exit editor mode</summary>
    public virtual void ReconstitutionMode(bool enter)
    {
        if (!editC)
            editC = GetComponent<Collider>();
        if (editC)
            editC.enabled = enter;
    }
}
