using HitBoxLib.HitBoxSegment;
using System.Text.Json.Serialization;
using System.Text.Json;
using DataPipes.DTO;
using DataPipes.DTO.Register;
using HitBoxLib.Segment.SignsTypeSide;


namespace HitBoxLib.Segment.DTOs;
/// <summary>
/// Data Transfer Object (DTO) for the HitBox class.
/// Represents the main hitbox and a list of segmented hitboxes,
/// used for serialization and deserialization purposes.
/// </summary>
public class HitBoxDTO : IDTO<HitBox>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();

    [JsonIgnore]
    private HitBox hitBox = new();

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "hitBox_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";

    /// <summary>
    /// The main hitbox represented as a BoxDTO.
    /// </summary>
    public BoxDTO MainHitBox = new();

    /// <summary>
    /// List of segmented hitboxes represented as BoxDTOs.
    /// </summary>
    public List<BoxDTO> SegmentedHitbox = new();


    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    public HitBoxDTO() { }

    /// <summary>
    /// Constructor that initializes the DTO with an existing HitBox object.
    /// </summary>
    /// <param name="hitBox">The HitBox instance to convert to DTO.</param>
    public HitBoxDTO(HitBox hitBox)
    {
        this.hitBox = hitBox;

        MainHitBox = new BoxDTO(hitBox.MainHitBox);
        foreach (var segmnet in hitBox.SegmentedHitbox)
            SegmentedHitbox.Add(new BoxDTO(segmnet));
    }

    /// <summary>
    /// Converts the internal HitBox data into the DTO format,
    /// mapping the main hitbox and segmented hitboxes into BoxDTOs.
    /// </summary>
    public void ToDTO()
    {
        MainHitBox.ToDTO();
        foreach (var segmnet in SegmentedHitbox)
            segmnet.ToDTO();
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
        BoxDTO.RegisterDTOJsonConverters(options);
    }


    /// <summary>
    /// Deserializes a JSON file into a HitBoxDTO and converts it back into a HitBox object.
    /// Throws an exception if the JSON file path is invalid.
    /// </summary>
    /// <param name="dto">DTO.</param>
    public static HitBox ToObject(HitBoxDTO dto)
    {
        HitBox hitBox = new HitBox();

        hitBox.MainHitBox = dto.MainHitBox.ToObject();
        foreach (var segment in dto.SegmentedHitbox)
            hitBox.AddSegmentHitBox(segment.ToObject());

        return hitBox;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public HitBox ToObject()
    {
        HitBox hitBox = new HitBox();

        hitBox.MainHitBox = MainHitBox.ToObject();
        foreach (var segment in SegmentedHitbox)
            hitBox.AddSegmentHitBox(segment.ToObject());

        return hitBox;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(HitBox target)
    {
        target.MainHitBox = MainHitBox.ToObject();
        foreach (var segment in SegmentedHitbox)
            target.AddSegmentHitBox(segment.ToObject());
    }
}