shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE:hint_screen_texture;
uniform float hue_rotation : hint_range(0,1) = 0.0;

vec4 screen(vec4 base, vec4 blend) {
  return 1.0 - (1.0 - base) * (1.0 - blend);
}

vec3 okhsl_rotate(vec3 color, float angle) {
  vec3 okhsl = mat3(vec3(0.299, 1.0, 0.40462981), vec3(0.587, -0.46081557, -1.0), vec3(0.114, -0.53918443, 0.59537019)) * color;
  okhsl.yz = mat2(vec2(cos(angle), sin(angle)), vec2(-sin(angle), cos(angle))) * okhsl.yz;
  return mat3(vec3(1.0, 1.0, 1.0), vec3(0.5696804, -0.1620848, -0.6590654), vec3(0.3235513, -0.3381869, 0.8901581)) * okhsl;
}

void fragment() {
  vec4 bg_color = texture(SCREEN_TEXTURE, SCREEN_UV);
  COLOR.rgb = screen(bg_color, COLOR).rgb;
  COLOR.rgb = okhsl_rotate(COLOR.rgb, TAU * hue_rotation);
}