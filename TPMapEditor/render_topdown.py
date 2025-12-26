import bpy
import sys
import math
import os

# ------------------------------------------------------
# ARGS
# ------------------------------------------------------
argv = sys.argv
argv = argv[argv.index("--") + 1:]

input_obj = argv[0]
output_png = argv[1]
PIXELS_PER_UNIT = int(argv[2]) if len(argv) > 2 else 1

# ------------------------------------------------------
# SCENE RESET
# ------------------------------------------------------
bpy.ops.wm.read_factory_settings(use_empty=True)

# ------------------------------------------------------
# OBJ IMPORT
# ------------------------------------------------------

bpy.ops.wm.obj_import(filepath=input_obj) # Blender 4.5.5
# bpy.ops.import_scene.obj(filepath=input_obj) # Blender 2.93

imported_objects = [
    obj for obj in bpy.context.selected_objects
    if obj.type == 'MESH'
]

if not imported_objects:
    raise Exception("❌ No mesh found in the OBJ")

# ------------------------------------------------------
# APPLY TRANSFORMS
# ------------------------------------------------------

for obj in imported_objects:
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)

# ------------------------------------------------------
# BOUNDING BOX
# ------------------------------------------------------

all_vertices = []

for obj in imported_objects:
    for v in obj.data.vertices:
        all_vertices.append(obj.matrix_world @ v.co)

min_x = min(v.x for v in all_vertices)
max_x = max(v.x for v in all_vertices)
min_y = min(v.y for v in all_vertices)
max_y = max(v.y for v in all_vertices)
min_z = min(v.z for v in all_vertices)
max_z = max(v.z for v in all_vertices)

size_x = max_x - min_x
size_y = max_y - min_y

center_x = (min_x + max_x) / 2
center_y = (min_y + max_y) / 2

pivot_norm_x = (obj.location.x - min_x) / (max_x - min_x)
pivot_norm_y = (obj.location.y - min_y) / (max_y - min_y)

# ------------------------------------------------------
# CAMERA ORTHO TOP-DOWN
# ------------------------------------------------------
cam_data = bpy.data.cameras.new("TopCamera")
cam_data.type = 'ORTHO'
cam_data.ortho_scale = max(size_x, size_y)

cam = bpy.data.objects.new("TopCamera", cam_data)
cam.location = (center_x, center_y, max_z)
cam.rotation_euler = (0, 0, 0)

bpy.context.scene.collection.objects.link(cam)
bpy.context.scene.camera = cam

# ------------------------------------------------------
# LIGHT
# ------------------------------------------------------
light_data = bpy.data.lights.new(name="Sun", type='SUN')
light_data.energy = 3

light = bpy.data.objects.new(name="Sun", object_data=light_data)
light.location = (center_x, center_y, max_z + 20)

bpy.context.scene.collection.objects.link(light)

# ------------------------------------------------------
# RENDER SETTINGD
# ------------------------------------------------------

# res_x = int(size_x * PIXELS_PER_UNIT)
# res_x = int(size_x * PIXELS_PER_UNIT)
res_x = int(size_x)
res_y = int(size_y)

scene = bpy.context.scene

# GPU
scene.render.engine = 'BLENDER_WORKBENCH'
wb = scene.display
sh = wb.shading

sh.color_type = 'TEXTURE'
sh.light = 'STUDIO'

sh.show_shadows = True
sh.shadow_intensity = 0
sh.show_specular_highlight = False

scene.view_settings.view_transform = 'Standard'
scene.view_settings.exposure = 1.5
scene.view_settings.gamma = 1.0

# CYCLES if you have GPU issues like me :)

# scene.render.engine = 'CYCLES'
# scene.cycles.device = 'CPU'

# scene.cycles.samples = 1
# scene.cycles.max_bounces = 0
# scene.cycles.diffuse_bounces = 0
# scene.cycles.glossy_bounces = 0
# scene.cycles.transparent_max_bounces = 0
# scene.cycles.volume_bounces = 0

# ------------------------------------------------------
# RENDER
# ------------------------------------------------------

scene.render.resolution_x = res_x
scene.render.resolution_y = res_y

scene.render.film_transparent = True
scene.render.image_settings.file_format = 'PNG'
scene.render.filepath = output_png

bpy.ops.render.render(write_still=True)

print("✅ Rendu terminé :", output_png)

# ------------------------------------------------------
# DPI FIX (FOR WPF) AND RESIZE
# ------------------------------------------------------

from PIL import Image

img = Image.open(output_png)
width = img.size[0] * PIXELS_PER_UNIT
height = img.size[1] * PIXELS_PER_UNIT
img_resized = img.resize((width, height), Image.Resampling.LANCZOS)
img_resized.save(output_png, dpi=(96, 96))

# ------------------------------------------------------
# PIVOT EXPORT FOR 2D ROTATION
# ------------------------------------------------------

import xml.etree.ElementTree as ET
import os

# xml_path = os.path.join(os.path.dirname(output_png), "WorldObjects.xml")

# if os.path.exists(xml_path):
    # tree = ET.parse(xml_path)
    # root = tree.getroot()
# else:
    # root = ET.Element("Objects")
    # tree = ET.ElementTree(root)

# obj_node = ET.SubElement(root, "Object")

xml_path = output_png.replace(".png", ".xml")

obj_node = ET.Element("Object")

ET.SubElement(obj_node, "Name").text = os.path.splitext(os.path.basename(input_obj))[0]
ET.SubElement(obj_node, "PivotX").text = str(round(pivot_norm_x, 6))
ET.SubElement(obj_node, "PivotY").text = str(round(1-pivot_norm_y, 6))

tree = ET.ElementTree(obj_node)

tree.write(xml_path, encoding="utf-8", xml_declaration=True)
