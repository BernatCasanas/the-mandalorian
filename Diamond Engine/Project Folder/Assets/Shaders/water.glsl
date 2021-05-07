#ifdef vertex
#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoords;

out vec3 pos;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;
uniform float time;

float speed = 0.005;
float wave_length = 0.075;
float steepness = 0.5;

vec3 direction_1 = vec3(0.35, 0.315, -0.35);
vec3 direction_2 = vec3(-0.25, 0.2, 0.25);
vec3 direction_3 = vec3(0.15, -0.2, 0.15);

out float relative_position;

out VS_OUT {
    vec4 clipSpace;
    vec3 FragPos;
    vec2 TexCoords;
    vec3 TangentLightPos;
    vec3 TangentViewPos;
    vec3 TangentFragPos;
} vs_out;

float Random(vec2 coord)  //Canonical random function
{
 return fract(sin(dot(coord, vec2(12.9898, 78.233))) * 43758.5453);
}


float Generate_Perlin_Noise(vec2 coord)
{
 vec2 i = floor(coord);
 vec2 f = fract(coord);
 
 float tl = Random(i);
 float tr = Random(i + vec2(1.0, 0.0));
 float bl = Random(i + vec2(0.0, 1.0));
 float br = Random(i + vec2(1.0, 1.0));
 
 vec2 tl_vec = vec2(-sin(tl), cos(tl));
 vec2 tr_vec = vec2(-sin(tr), cos(tr));
 vec2 bl_vec = vec2(-sin(bl), cos(bl));
 vec2 br_vec = vec2(-sin(br), cos(br));
 
 float tl_dot = dot(tl_vec, f);
 float tr_dot = dot(tr_vec, f - vec2(1.0, 0.0));
 float bl_dot = dot(bl_vec, f - vec2(0.0, 1.0));
 float br_dot = dot(br_vec, f - vec2(1.0, 1.0));
 
 vec2 cubic = f * f * (3.0 - 2.0 * f);
 
 float top_mix = mix(tl_dot, tr_dot, cubic.x);
 float bot_mix = mix(bl_dot, br_dot, cubic.x);
 
 return mix(top_mix, bot_mix, cubic.y);
}

vec3 generateWave(float amp, vec3 direction, float num_waves, float steepness,
 float phase_constant, float w ) {
 vec3 dir = normalize(direction);
 float q = steepness / (w * amp * num_waves);
 
 vec3 wave;
 wave.x = q * amp * dir.x * cos(dot(w * dir, position) + phase_constant * time);
 wave.z = q * amp * dir.z * cos(dot(w * dir, position) + phase_constant * time);
 wave.y = amp * sin(dot(w * dir, position) + phase_constant * time);
 
 return wave;
}

void main()
{
 vec3 fPosition = position;
 float pi = 3.1415f;
 float phase_constant = speed * 2.0 * pi / wave_length;
 float w = sqrt(9.81 * (2 * pi / wave_length));
 float num_waves = 3.0;
 
 float amp1 = 0.2;
 vec3 wave1 = generateWave(amp1, direction_1, num_waves, steepness, phase_constant, w);
 
 float amp2 = 0.15;
 vec3 wave2 = generateWave(amp2, direction_2, num_waves, steepness, phase_constant, w);
 
 float amp3 = 0.2;
 vec3 wave3 = generateWave(amp3, direction_3, num_waves, steepness, phase_constant, w);
 
 fPosition += wave1 + wave2 + wave3;
 relative_position = fPosition.y / ((amp1 * 2.0 + amp2 * 2.0 + amp3 * 2.0) / num_waves);
 relative_position = (relative_position + 0.5) * 0.5;
 relative_position = max(relative_position, -0.2);
 
 vs_out.TexCoords = texCoords;
 vs_out.FragPos = vec3(model_matrix * vec4(position, 1.0));
 
 vs_out.clipSpace = projection * view * model_matrix * vec4(fPosition, 1.0);
 gl_Position = projection * view * model_matrix * vec4(fPosition, 1.0);
 pos = fPosition;
}

#endif

#ifdef fragment
#version 330 core
in vec3 pos;
in float relative_position;
out vec4 color;

uniform float time;

in VS_OUT {
	vec4 clipSpace;
    vec3 FragPos;
    vec2 TexCoords;
    vec3 TangentLightPos;
    vec3 TangentViewPos;
    vec3 TangentFragPos;
} fs_in;

uniform vec2 water_direction;
vec3 water_color = vec3(0.18, 0.36, 0.9);
vec3 ripple_color = vec3(0.45, 0.7, 0.95);

float Random(vec2 coord)  //Canonical random function
{
 return fract(sin(dot(coord, vec2(12.9898, 78.233))) * 43758.5453);
}


vec2 Random2(vec2 coord)
{
 return fract(sin( vec2( dot(coord, vec2(127.1,311.7)), dot(coord, vec2(269.5,183.3)))) * 43758.5453);
}


float Generate_Cellular_Noise(vec2 coord)
{
 vec2 i = floor(coord);
 vec2 f = fract(coord);
 
 float min_distance = 999999.0;
 
 for (float x = -1.0; x <= 1.0; x++){
   for (float y = -1.0; y <= 1.0; y++){
     
     vec2 node = Random2(i + vec2(x, y)) + vec2(x, y);
     float distance = sqrt((f - node).x * (f - node).x + (f - node).y * (f - node).y);
   
     min_distance = min(min_distance, distance);
    }
  }
 return min_distance;
}


float Generate_Fractal_Cellular_Noise(vec2 coord)
{
 int OCTAVES = 2;
 float value = 0.0;
 float scale = 0.2;
 
 for (int i = 0; i < OCTAVES; i++) {
   
   value += Generate_Cellular_Noise(coord) * scale;
   coord *= 2.0;
   scale *= 0.5;
 }

return value;
}


float Generate_Perlin_Noise(vec2 coord)
{
 vec2 i = floor(coord);
 vec2 f = fract(coord);
 
 float tl = Random(i);
 float tr = Random(i + vec2(1.0, 0.0));
 float bl = Random(i + vec2(0.0, 1.0));
 float br = Random(i + vec2(1.0, 1.0));
 
 vec2 tl_vec = vec2(-sin(tl), cos(tl));
 vec2 tr_vec = vec2(-sin(tr), cos(tr));
 vec2 bl_vec = vec2(-sin(bl), cos(bl));
 vec2 br_vec = vec2(-sin(br), cos(br));
 
 float tl_dot = dot(tl_vec, f);
 float tr_dot = dot(tr_vec, f - vec2(1.0, 0.0));
 float bl_dot = dot(bl_vec, f - vec2(0.0, 1.0));
 float br_dot = dot(br_vec, f - vec2(1.0, 1.0));
 
 vec2 cubic = f * f * (3.0 - 2.0 * f);
 
 float top_mix = mix(tl_dot, tr_dot, cubic.x);
 float bot_mix = mix(bl_dot, br_dot, cubic.x);
 
 return mix(top_mix, bot_mix, cubic.y);
}


float Generate_Fractal_Perlin_Noise(vec2 coord)
{
 int OCTAVES = 4;
 float value = 0.0;
 float scale = 0.5;
 
 for (int i = 0; i < OCTAVES; i++) {
   
   value += Generate_Perlin_Noise(coord) * scale;
   coord *= 2.0;
   scale *= 0.5;
 }

return value;
}


void main()
{
 vec2 motion = vec2(Generate_Fractal_Perlin_Noise(vec2(pos.xz) + water_direction));
 float value = Generate_Fractal_Cellular_Noise(vec2(pos.xz) + motion * 0.5);
 ripple_color *= value * relative_position;
 
 water_color =  water_color + ripple_color * 3.5;
 color = vec4(water_color, 1.0);
}

#endif







