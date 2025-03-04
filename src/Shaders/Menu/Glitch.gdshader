shader_type canvas_item;

uniform float time_speed : hint_range(0.1, 5.0) = 0.5;
uniform float glitch_intensity : hint_range(0.0, 10.0) = 0.6;
uniform float chromatic_offset : hint_range(0.0, 0.1) = 0.002;
uniform float strength : hint_range(0.0, 1.0) = 1.0;

float rand(vec2 uv) {
    return fract(sin(dot(uv.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

void fragment() {
    float time_offset = TIME * time_speed;
    float glitch = sin(time_offset * 10.0) * cos(time_offset * 15.0);
    glitch *= step(0.5, fract(time_offset * 3.0)) * glitch_intensity * 0.02;

    vec2 uv = UV;
    float dynamic_chromatic_offset = chromatic_offset * glitch_intensity;

    float r_offset = 0.02 * (rand(uv + time_offset) - 0.5) + dynamic_chromatic_offset;
    float g_offset = glitch * (rand(uv + time_offset * 1.1) - 0.5);
    float b_offset = glitch * (rand(uv + time_offset * 1.2) - 0.5) - dynamic_chromatic_offset;

    vec4 tex_color_r = texture(TEXTURE, clamp(uv + vec2(r_offset, 0.0), vec2(0.0), vec2(1.0)));
    vec4 tex_color_g = texture(TEXTURE, clamp(uv + vec2(g_offset, 0.0), vec2(0.0), vec2(1.0)));
    vec4 tex_color_b = texture(TEXTURE, clamp(uv + vec2(b_offset, 0.0), vec2(0.0), vec2(1.0)));

    vec4 tex_color;
    tex_color.r = tex_color_r.r * strength;
    tex_color.g = tex_color_g.g * strength;
    tex_color.b = tex_color_b.b * strength;
    tex_color.a = (tex_color_r.a + tex_color_g.a + tex_color_b.a) / 3.0;

    COLOR = tex_color;
}