#ifdef vertex
#version 330 core
layout (location = 0) in vec3 position;
out vec3 pos;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;
uniform float time;


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



void main()
{
  pos = position;
  pos.y += Generate_Perlin_Noise(vec2(pos.x, pos.z) + time * 0.5);
 
  gl_Position = projection * view * model_matrix * vec4(pos, 1.0f);

}

#endif

#ifdef fragment
#version 330 core
in vec3 pos;

out vec4 color;

uniform float time;

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
 vec2 motion = vec2(Generate_Fractal_Perlin_Noise(vec2(pos.x, pos.z) + time + water_direction));
 float value = Generate_Fractal_Cellular_Noise(vec2(pos.x * 7, pos.z * 7) + motion * 0.5);
 ripple_color *= value;
 
 water_color =  water_color + ripple_color;
 color = vec4(water_color, 1.0);
}

#endif


