#version 330 core

out vec4 FragColor;

in vec3 FragPos;  // Fragment position in world space
in vec3 Normal;   // Normal vector passed from the vertex shader

uniform vec3 lightPos;  // Position of the light
uniform vec3 lightColor; // Light color
uniform vec3 objectColor; // Color of the object (terrain)
uniform vec3 viewPos;  // Add this line to declare viewPos as a uniform

void main()
{
    // Ambient lighting
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    // Diffuse lighting
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    // Specular lighting
    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;

    // Combine lighting
    vec3 result = (ambient + diffuse + specular) * objectColor;
    FragColor = vec4(result, 1.0);
}