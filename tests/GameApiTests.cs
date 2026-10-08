using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// The game members the patches rely on, checked against the game DLL: a game update that renames or retypes one fails
/// here (CI builds against the dedicated server's DLLs) instead of silently in game.
/// </summary>
internal static partial class Tests
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    private static void Field(Type type, string name, Type fieldType, BindingFlags flags = Instance)
    {
        var field = type.GetField(name, flags);
        True(field != null, $"{type.Name}.{name} exists");
        Eq(fieldType, field.FieldType, $"{type.Name}.{name} type");
    }

    private static MethodInfo Method(Type type, string name)
    {
        var methods = type.GetMethods(Instance | BindingFlags.Static).Where(m => m.Name == name && m.DeclaringType == type).ToArray();
        Eq(1, methods.Length, $"{type.Name}.{name}: one method (Harmony patches it by name)");
        return methods[0];
    }

    private static void Param(MethodInfo method, string name, Type type)
    {
        var p = method.GetParameters().FirstOrDefault(x => x.Name == name);
        True(p != null, $"{method.DeclaringType.Name}.{method.Name} has '{name}'");
        Eq(type, p.ParameterType, $"{method.DeclaringType.Name}.{method.Name} '{name}' type");
    }

    private static void Test_Game_TheShip()
    {
        var tick = Method(typeof(Ship), "CustomFixedUpdate");
        True(tick.IsPublic, "Ship.CustomFixedUpdate is public (nameof)");
        Param(tick, "fixedDeltaTime", typeof(float));
        Field(typeof(Ship), "m_nview", typeof(ZNetView));          // Harmony ___m_nview
        Field(typeof(Ship), "m_body", typeof(Rigidbody));          // Harmony ___m_body
        Field(typeof(Ship), "m_shipControlls", typeof(ShipControlls));
        Field(typeof(Ship), "m_sailForceFactor", typeof(float));
        Field(typeof(Ship), "m_waterLevelOffset", typeof(float));
        Field(typeof(Ship), "m_disableLevel", typeof(float));
        Eq(typeof(Ship.Speed), Method(typeof(Ship), "GetSpeedSetting").ReturnType, "Ship.GetSpeedSetting()");
        Eq(typeof(bool), Method(typeof(Ship), "IsOwner").ReturnType, "Ship.IsOwner()");
        var names = Enum.GetNames(typeof(Ship.Speed));
        Eq("Stop,Back,Slow,Half,Full", string.Join(",", names), "Ship.Speed gears");
    }

    private static void Test_Game_TheHold()
    {
        Eq(typeof(Inventory), Method(typeof(Container), "GetInventory").ReturnType, "Container.GetInventory()");
        var count = Method(typeof(Inventory), "CountItems");
        Param(count, "name", typeof(string));
        Param(count, "matchWorldLevel", typeof(bool));
        var remove = typeof(Inventory).GetMethods(Instance).Single(m => m.Name == "RemoveItem"
            && m.GetParameters().Length == 4 && m.GetParameters()[0].ParameterType == typeof(string));
        Param(remove, "amount", typeof(int));
        Param(remove, "worldLevelBased", typeof(bool));
        Field(typeof(ZDOVars), "s_inUse", typeof(int), Static);
    }

    private static void Test_Game_OwnershipAndWater()
    {
        True(Method(typeof(ZNetView), "ClaimOwnership").GetParameters().Length == 0, "ZNetView.ClaimOwnership()");
        Eq(typeof(long), Method(typeof(ZDO), "GetOwner").ReturnType, "ZDO.GetOwner()");
        Eq(typeof(long), Method(typeof(ShipControlls), "GetUser").ReturnType, "ShipControlls.GetUser()");
        Eq(typeof(bool), Method(typeof(ShipControlls), "HaveValidUser").ReturnType, "ShipControlls.HaveValidUser()");
        Eq(typeof(bool), Method(typeof(Container), "IsInUse").ReturnType, "Container.IsInUse()");
        Eq(typeof(long), typeof(Player).GetMethod("GetPlayerID", Type.EmptyTypes)?.ReturnType, "Player.GetPlayerID()");
        var water = Method(typeof(Floating), "GetWaterLevel");
        True(water.IsStatic, "Floating.GetWaterLevel is static");
        Param(water, "p", typeof(Vector3));
        Param(water, "previousAndOut", typeof(WaterVolume).MakeByRefType());
    }
}
