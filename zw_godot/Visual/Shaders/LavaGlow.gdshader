shader_type spatial;

uniform sampler2D texture_alpha;

void fragment() {
	ALPHA = 0.1;
	ALBEDO = vec3(0.7, 0.2, 0.1);
}
