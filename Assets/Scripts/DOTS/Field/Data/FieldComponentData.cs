using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

using GuildID = System.Int64;

//--------------------------------------------------------------------
// FLEET
//--------------------------------------------------------------------
// MATERIAL
public enum EnumFleetTrait      
{
    Single  = 0,
    Joint,                  // ���� ����
    JointMember,            // ���� ������
};

// COLOR
public enum EnumFleetRelation   
{
    InvaderEvent = 0,       // Event
    MyFleet,                // �� �δ� 
    MyGuildFleet,           // �� ���Ϳ� �δ� 
    NotMyGuildFleet,        // �� �δ뵵 �ƴϰ� �� ���Ϳ��� �ƴ� �δ�
    Unknown
};

//--------------------------------------------------------------------
// AREA
//--------------------------------------------------------------------
// MATERIAL
public enum EnumAreaStatus      
{
    Placing = 0,            // �õ���
    Constructing,           // ������
    Completed,              // �ϼ��� 
};

// COLOR
public enum EnumAreaOwn         
{
    Ally    = 0,            // �Ʊ�
    Enemy,                  // ����
    City                    // ����
};

// MESH LINE TYPE
public enum EnumdMeshLineType
{
    FleetLine   = 0,    // �δ� ���� ����
    AreaRegion,         // ���� ����

    MAX,
};

//--------------------------------------------------------------------
// Enum : FieldMeshLineData
// Desc : LINE MESH �� �����ϱ����� ������
//--------------------------------------------------------------------
public struct FieldMeshLineData : IComponentData
{
    public Int64                m_ID;
    public EnumdMeshLineType    m_Type;
    public Int16                m_Material;
    public Int16                m_Color;
    public float                m_Speed;
};

//--------------------------------------------------------------------
// Struct: AreaElement 
// Desc  : Area Point Array
//--------------------------------------------------------------------

[InternalBufferCapacity(5)]
public struct AreaElement : IBufferElementData
{
    public GuildID              m_GuildID;      // GUILD ID
    public Int64                m_Priority;     // �켱����
    public RectInt              m_Rect;         // ���� GRID 

    public AreaElement(GuildID guildId, Int64 priority, RectInt rect)
    {
        m_GuildID   = guildId;      // GUILD ID
        m_Priority  = priority;     // �켱����
        m_Rect      = rect;         // ���� GRID 
    }
}

//--------------------------------------------------------------------
// Struct: GuildIAreaElement 
// Desc  : Guild ���� Array
//--------------------------------------------------------------------
[InternalBufferCapacity(5)]
public struct GuildAreaElement : IBufferElementData
{
    public Int64                m_GroupID;
    public GuildID              m_GuildID;
    public EnumAreaStatus       m_Status;
    public EnumAreaOwn          m_Own;

    public GuildAreaElement(Int64 groupId, GuildID guildId, EnumAreaStatus status, EnumAreaOwn own)
    {
        m_GroupID   = groupId;      // GROUP ID
        m_GuildID   = guildId;
        m_Status    = status;
        m_Own       = own;
    }
}


//--------------------------------------------------------------------
// Struct: FieldMeshPointElement 
// Desc  : Mesh Point Array
//--------------------------------------------------------------------
[InternalBufferCapacity(5)]
public struct FieldMeshPointElement : IBufferElementData
{
    public float2 Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator float2(FieldMeshPointElement e)
    {
        return e.Value;
    }

    public static implicit operator FieldMeshPointElement(float2 e)
    {
        return new FieldMeshPointElement { Value = e };
    }
    public static implicit operator FieldMeshPointElement(Vector2 e)
    {
        return new FieldMeshPointElement { Value = e };
    }
    public static implicit operator FieldMeshPointElement(int2 e)
    {
        return new FieldMeshPointElement { Value = e };
    }
}


//--------------------------------------------------------------------
// Struct: FieldMeshVertexElement
// Desc  : Mesh Vertex Array
//--------------------------------------------------------------------
[InternalBufferCapacity(5)]
public struct FieldMeshVertexElement : IBufferElementData
{
    public float3 Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator Vector3(FieldMeshVertexElement e)
    {
        return e.Value;
    }

    public static implicit operator FieldMeshVertexElement(float3 e)
    {
        return new FieldMeshVertexElement { Value = e };
    }
}


//--------------------------------------------------------------------
// Struct: FieldMeshTriangleElement 
// Desc  : Mesh Index Array
//--------------------------------------------------------------------
[InternalBufferCapacity(5)]
public struct FieldMeshTriangleElement : IBufferElementData
{
    public int Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator int(FieldMeshTriangleElement e)
    {
        return e.Value;
    }

    public static implicit operator FieldMeshTriangleElement(int e)
    {
        return new FieldMeshTriangleElement { Value = e };
    }
}

//--------------------------------------------------------------------
// Struct: FieldMeshNormalElement 
// Desc  : Mesh Normal Array
//--------------------------------------------------------------------
[InternalBufferCapacity(5)]
public struct FieldMeshNormalElement : IBufferElementData
{
    public float3 Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator float3(FieldMeshNormalElement e)
    {
        return e.Value;
    }

    public static implicit operator FieldMeshNormalElement(float3 e)
    {
        return new FieldMeshNormalElement { Value = e };
    }
}


//--------------------------------------------------------------------
// Struct: FieldMeshUVElement 
// Desc  : Mesh UV Array
//--------------------------------------------------------------------
[InternalBufferCapacity(5)]
public struct FieldMeshUVElement : IBufferElementData
{
    public float2 Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator float2(FieldMeshUVElement e)
    {
        return e.Value;
    }
    public static implicit operator Vector2(FieldMeshUVElement e)
    {
        return e.Value;
    }

    public static implicit operator FieldMeshUVElement(float2 e)
    {
        return new FieldMeshUVElement { Value = e };
    }
}


//--------------------------------------------------------------------
// Struct: FieldMeshColorElement 
// Desc  : Mesh Vertex Color Array
//--------------------------------------------------------------------
[InternalBufferCapacity(5)]
public struct FieldMeshColorElement : IBufferElementData
{
    public float4 Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator float4(FieldMeshColorElement e)
    {
        return e.Value;
    }
    public static implicit operator Color(FieldMeshColorElement e)
    {
        return (Vector4)e.Value;
    }

    public static implicit operator FieldMeshColorElement(float4 e)
    {
        return new FieldMeshColorElement { Value = e };
    }

    public static implicit operator FieldMeshColorElement(Color e)
    {
        return new FieldMeshColorElement { Value = (Vector4)e };
    }
}



