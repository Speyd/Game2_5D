using DataPipes.DTO.Register;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using System.Text.Json;
using System.Text.Json.Serialization;
using DataPipes.DTO;


namespace HitBoxLib.Segment.DTOs;
/// <summary>
/// Data Transfer Object (DTO) for the HitBoxSide class.
/// Contains information about the coordinate plane, side indices, offset, and side size.
/// Used for serialization and deserialization of HitBoxSide objects.
/// </summary>
public class HitBoxSideDTO : IDTO<HitBoxSide>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private HitBoxSide hitBoxSide;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "hitBoxSide_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";

    /// <summary>
    /// The coordinate plane of the hitbox side.
    /// </summary>
    public CoordinatePlane CoordinatePlane;

    /// <summary>
    /// The side index.
    /// </summary>
    public int Side;

    /// <summary>
    /// The original side index.
    /// </summary>
    public int OrginalSide;

    /// <summary>
    /// The offset value for the side.
    /// </summary>
    public int Offset;

    /// <summary>
    /// The size of the side.
    /// </summary>
    public SideSize SideSize;

    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    public HitBoxSideDTO()
    { }
    public HitBoxSideDTO(HitBoxSide hitBoxSide)
    {
        this.hitBoxSide = hitBoxSide;
    }

    /// <summary>
    /// Converts a HitBoxSide instance into this DTO representation.
    /// </summary>
    public void ToDTO()
    {
        CoordinatePlane = hitBoxSide.CoordinatePlane;
        Side = (int)hitBoxSide.Side;
        OrginalSide = (int)hitBoxSide.OrginalSide;
        Offset = (int)hitBoxSide.Offset;
        SideSize = hitBoxSide.SideSize;
    }


    /// <summary>
    /// Ensures that the DTO registration logic is initialized once per application domain.
    /// Typically used to prepare internal mappings or caches for DTOs.
    /// </summary>
    public static void RegisterDTO() { }
    /// <summary>
    /// Registers custom JSON converters required for correct serialization and deserialization of DTO types.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to which converters will be added.</param>
    public static void RegisterDTOJsonConverters(JsonSerializerOptions options)
    {
        DtoTypeRegistry.RegisterAllDTOJsonConverters(options);
    }



    /// <summary>
    /// Converts this DTO back into a HitBoxSide instance.
    /// </summary>
    /// <returns>A tuple containing the key (CoordinatePlane, SideSize) and the reconstructed HitBoxSide object.</returns>
    public static HitBoxSide ToObject(HitBoxSideDTO dto)
    {
        return new HitBoxSide(dto.CoordinatePlane, dto.Side, dto.OrginalSide, dto.Offset, dto.SideSize);
    }

    /// <summary>
    /// Converts this DTO back into a HitBoxSide instance.
    /// </summary>
    /// <returns>A tuple containing the key (CoordinatePlane, SideSize) and the reconstructed HitBoxSide object.</returns>
    public ((CoordinatePlane, SideSize) key, HitBoxSide value) Deserialization()
    {
        HitBoxSide hitBoxSide = new HitBoxSide(CoordinatePlane, Side, OrginalSide, Offset, SideSize);
        return ((CoordinatePlane, SideSize), hitBoxSide);
    }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public HitBoxSide ToObject()
    {
        return new HitBoxSide(CoordinatePlane, Side, OrginalSide, Offset, SideSize);
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(HitBoxSide target)
    {
        target = new HitBoxSide(CoordinatePlane, Side, OrginalSide, Offset, SideSize);
    }
}