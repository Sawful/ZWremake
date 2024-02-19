shader_type spatial;

void fragment() {
	vec3 pixelColor = vec3(0.9, 0.1, 0.1);
	ALBEDO = pixelColor;
	ROUGHNESS = 0.5;
	METALLIC = 0.5;
}
