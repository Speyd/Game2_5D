#version 330 core

uniform vec2 u_screenSize;      // Size screen
uniform vec2 u_playerPos;       // Position of player
uniform vec2 u_playerDir;       // Player direction
uniform vec2 u_playerPlane;     // Camera plane
uniform sampler2D u_texture;        // Floor texture
uniform float u_verticalAngle;      // Player's angle
uniform int u_Raising;     // Raises the wall to a certain height, multiplied by the player's position
uniform float u_textureScale;       //Increase texture size
uniform float u_DivisionCoef;      //Basic screen size divider (initially half)
uniform float u_normalAngleGreaterZero;     //Normalization of the divider when lowering the chamber    
uniform float u_maxDivisionCoef;       //Maximum divisor   


out vec4 FragColor;         //Changeable color

void main()
{

    // Pixel coordinates
    vec2 fragCoord = gl_FragCoord.xy;

    //halfScreenHeight
    float halfHeight = u_screenSize.y /  u_DivisionCoef;
    if(u_verticalAngle > 0)
        halfHeight = u_screenSize.y / (u_DivisionCoef + min(u_verticalAngle / u_normalAngleGreaterZero, u_maxDivisionCoef));

    if(fragCoord.y >= halfHeight * (1 + u_verticalAngle))
    {
        float multTexture = u_verticalAngle < 0 ? 1 : -1;

        // Calculating the distance to a row
        float rowDistance = abs((u_screenSize.y / (fragCoord.y - halfHeight * (1 + u_verticalAngle)))); 

        //Steps to move on the floor that depend on the player's direction
        vec2 rayDirLeft = (u_playerDir - u_playerPlane) * -(1 - u_verticalAngle * multTexture); // Left vector for movement on the floor
        vec2 rayDirRight = (u_playerDir + u_playerPlane) * -(1 - u_verticalAngle * multTexture); // Right vector for movement on the floor


        // Step on the floor
        vec2 floorStep = rowDistance * (rayDirRight - rayDirLeft) / u_screenSize.x;

        // Initial floor position taking into account scale
        vec2 floor = (-u_playerPos * u_Raising) + rowDistance * rayDirLeft + fragCoord.x * floorStep;

        // Normalize texture coordinates to prevent texture stretching
        vec2 texCoords = vec2(fract(floor.x  * u_textureScale), fract(floor.y * u_textureScale));

        // Color from texture
        vec4 texColor = texture(u_texture, texCoords);

        FragColor = texColor;
   }
}