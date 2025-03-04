shader_type spatial;
render_mode unshaded;

uniform int vertical_sections = 21;
uniform float star_size = 0.01;
uniform float star_falloff = 4.0;
uniform float margin = 1.2;
uniform vec3 top_color: source_color;
uniform vec3 bottom_color: source_color;

vec2 get_partition(vec2 uv) {
	float y = uv.y * 2. - 1.;
	float section_y = ceil(abs(y) * float(vertical_sections)) / float(vertical_sections);
	float horizontal_sections = max(4., ceil(float(vertical_sections) * 4. * cos(section_y * PI / 2.)));
	horizontal_sections = horizontal_sections > 0. ? horizontal_sections : 1.;
	return vec2(uv.x * horizontal_sections, y * float(vertical_sections));
}

vec2 rand_vec2(vec2 xy) {
    float rand1 = fract(sin(dot(xy, vec2(11.9131, 81.2317))) * 57183.77193);
    float rand2 = fract(sin(dot(xy, vec2(16.8131, 91.2327))) * 37113.66193);
	return vec2(rand1, rand2) * (1. - margin * 2.) + margin;
}

float star_brightness(vec2 uv) {
	vec2 partitions = get_partition(uv);
	vec2 partition_index = floor(partitions);
	vec2 inner_coord = fract(partitions);
	vec2 star_coord = rand_vec2(partition_index);
	float dist = length(inner_coord - star_coord);
	return pow(star_size / dist, star_falloff);
}

void fragment() {
	vec2 uv = UV;
	float brightness = star_brightness(uv);

    float gradient_factor = uv.y;
    vec3 gradient = mix(bottom_color, top_color, gradient_factor);

    ALBEDO = gradient + vec3(brightness);
}