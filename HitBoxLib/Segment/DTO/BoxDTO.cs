using DataPipes.DTO.Register;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Operations;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
using System.Text.Json;
using System.Text.Json.Serialization;
using DataPipes.DTO;


namespace HitBoxLib.Segment.DTOs;
/// <summary>
/// Data Transfer Object (DTO) for the Box class.
/// Contains color information, height render mode, a collection of hitbox sides,
/// and a title for the box.
/// Used for serialization and deserialization purposes.
/// </summary>
public class BoxDTO : IDTO<Box>
{
    /// <summary>
    /// Gets the global or shared <see cref="DTOJsonOptionsRegistry"/> instance associated with the implementing type.
    /// This registry is used to track mappings between DTO types and their corresponding domain object types.
    /// </summary>
    [JsonIgnore]
    public static DTOJsonOptionsRegistry DtoTypeRegistry { get; } = new();


    [JsonIgnore]
    private Box box;

    /// <summary>
    /// The base file name used when generating a unique file name for serialization (e.g., "object_", "dto_").
    /// </summary>
    [JsonIgnore]
    public string BaseFileName { get; } = "box_";
    /// <summary>
    /// The file extension to use when generating the file name (e.g., ".json").
    /// </summary>
    [JsonIgnore]
    public string BaseExtension { get; } = ".json";

    /// <summary>
    /// Red component of the render color.
    /// </summary>
    public byte R;

    /// <summary>
    /// Green component of the render color.
    /// </summary>
    public byte G;

    /// <summary>
    /// Blue component of the render color.
    /// </summary>
    public byte B;

    /// <summary>
    /// Alpha (opacity) component of the render color.
    /// </summary>
    public byte A;

    /// <summary>
    /// The height rendering mode for the box.
    /// </summary>
    public RenderHeightMode HeightRenderMode;

    /// <summary>
    /// List of hitbox sides in the box.
    /// </summary>
    public List<HitBoxSideDTO> Body = new();

    /// <summary>
    /// The title or name of the box.
    /// </summary>
    public string Title = string.Empty;

    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    public BoxDTO()
    { }
    public BoxDTO(Box box)
    {
        this.box = box;
        foreach (var item in box.Body)
            Body.Add(new HitBoxSideDTO(item.Value));
    }

    /// <summary>
    /// Converts a Box instance into this DTO representation,
    /// copying color, height mode, title, and all hitbox sides.
    /// </summary>
    public void ToDTO()
    {
        R = box.RenderColor.R;
        G = box.RenderColor.G;
        B = box.RenderColor.B;
        A = box.RenderColor.A;

        HeightRenderMode = box.HeightRenderMode;
        Title = box.Title;

        foreach (var body in Body)
            body.ToDTO();
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
        HitBoxSideDTO.RegisterDTOJsonConverters(options);
    }

    /// <summary>
    /// Converts this DTO back into a Box instance,
    /// reconstructing color, height mode, title, and hitbox sides.
    /// </summary>
    /// <returns>The reconstructed Box object.</returns>
    public static Box ToObject(BoxDTO dto)
    {
        Dictionary<(CoordinatePlane, SideSize), HitBoxSide> body = new();
        foreach (var bodyDTO in dto.Body)
        {
            var key_val = bodyDTO.Deserialization();
            body.Add(key_val.key, key_val.value);
        }

        Box box = new Box(body, dto.Title);
        box.HeightRenderMode = dto.HeightRenderMode;
        box.RenderColor = new SFML.Graphics.Color(dto.R, dto.G, dto.B, dto.A);

        return box;
    }

    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public Box ToObject()
    {
        Dictionary<(CoordinatePlane, SideSize), HitBoxSide> body = new();
        foreach (var bodyDTO in Body)
        {
            var key_val = bodyDTO.Deserialization();
            body.Add(key_val.key, key_val.value);
        }

        Box box = new Box(body, Title);
        box.HeightRenderMode = HeightRenderMode;
        box.RenderColor = new SFML.Graphics.Color(R, G, B, A);

        return box;
    }
    /// <summary>
    /// Restores the original object from its DTO representation. Used after deserialization.
    /// </summary>
    public void ToObject(Box target)
    {
        Dictionary<(CoordinatePlane, SideSize), HitBoxSide> body = new();
        foreach (var bodyDTO in Body)
        {
            var key_val = bodyDTO.Deserialization();
            body.Add(key_val.key, key_val.value);
        }

        target = new Box(body, Title);
        box.HeightRenderMode = HeightRenderMode;
        box.RenderColor = new SFML.Graphics.Color(R, G, B, A);
    }
}