shader_type canvas_item;

uniform vec4 background_color : source_color;
uniform vec4 fog_color : source_color;
uniform float fog_intensity : hint_range(0.0, 1.0) = 0.5;
uniform float noise_scale : hint_range(0.1, 10.0) = 3.0;
uniform float speed : hint_range(0.0, 5.0) = 1.0;

float random(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

float noise(vec2 uv) {
    vec2 i = floor(uv);
    vec2 f = fract(uv);

    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

void fragment() {
    vec2 uv = UV * noise_scale;
    uv.y += TIME * speed;

    float n = noise(uv);
    float alpha = mix(0.0, 1.0, n) * fog_intensity;

    vec4 col = mix(background_color, fog_color, alpha);
    COLOR = col;
}