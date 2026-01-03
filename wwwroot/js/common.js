/* =========================================================
   VARIABLES GLOBALES
========================================================= */
window.engine = null;
window.scene = null;
window.canvas = null;
let bicicletaMesh = null;
const accesoriosMesh = {};
/* =========================================================
   UTILIDAD: dividir ruta del modelo
========================================================= */
function dividirRuta(url) {
    const lastSlash = url.lastIndexOf("/");
    return {
        rootUrl: url.substring(0, lastSlash + 1),
        fileName: url.substring(lastSlash + 1)
    };
}

/* =========================================================
   UTILIDAD: ajustar cámara al modelo
========================================================= */
function ajustarCamaraMesh(meshes) {
    if (!meshes || meshes.length === 0 || !scene.activeCamera) return;

    const root = meshes[0];
    const bounds = root.getHierarchyBoundingVectors(true);
    const center = BABYLON.Vector3.Center(bounds.min, bounds.max);

    scene.activeCamera.setTarget(center);

    const size = BABYLON.Vector3.Distance(bounds.min, bounds.max);
    scene.activeCamera.radius = size * 1.5;
}

/* =========================================================
   MOSTRAR MODELO 3D EN MODAL
========================================================= */
window.mostrarModelo3D = function (rutaModelo3D) {

    console.log("Ruta recibida:", rutaModelo3D);

    // limpiar eventos previos
    $('#modelo3DModal').off('shown.bs.modal hidden.bs.modal');

    // mostrar modal
    $('#modelo3DModal').modal('show');

    $('#modelo3DModal').on('shown.bs.modal', function () {

        canvas = document.getElementById("renderCanvas");

        engine = new BABYLON.Engine(canvas, true);
        scene = new BABYLON.Scene(engine);

        // color de fondo (evita fondo oscuro confuso)
        scene.clearColor = new BABYLON.Color4(0.93, 0.92, 0.88, 1);

        // cámara
        const camera = new BABYLON.ArcRotateCamera(
            "camera",
            -Math.PI / 2,
            Math.PI / 2.5,
            5,
            BABYLON.Vector3.Zero(),
            scene
        );
        camera.attachControl(canvas, true);

        // luces
        new BABYLON.HemisphericLight(
            "light",
            new BABYLON.Vector3(0, 1, 0),
            scene
        );

        const partes = dividirRuta(rutaModelo3D);

        BABYLON.SceneLoader.ImportMesh(
            "",
            partes.rootUrl,
            partes.fileName,
            scene,
            function (meshes) {
                console.log("Modelo cargado correctamente", meshes);
                scene.createDefaultEnvironment({
                    createGround: false,
                    createSkybox: false
                });
                ajustarCamaraMesh(meshes);
            },
            null,
            function (scene, message, exception) {
                console.error("Error al cargar el modelo:", message, exception);
            }
        );

        engine.runRenderLoop(function () {
            if (scene) {
                scene.render();
            }
        });
    });

    $('#modelo3DModal').on('hidden.bs.modal', function () {
        if (engine) {
            engine.dispose();
            engine = null;
            scene = null;
        }
    });
};



/* =========================================================
   TOASTR
========================================================= */
window.ShowToastr = function (type, message) {
    if (type === "success") {
        toastr.success(message, "Operación Correcta", { timeOut: 10000 });
    }
    if (type === "error") {
        toastr.error(message, "Operación Fallida", { timeOut: 10000 });
    }
};

/* =========================================================
   SWEETALERT
========================================================= */
window.ShowSwal = function (type, message) {
    Swal.fire(
        type === "success" ? "Éxito" : "Error",
        message,
        type
    );
};

/* =========================================================
   MODALES DE CONFIRMACIÓN
========================================================= */
window.MostrarModalConfirmacionBorrado = function () {
    $('#modalConfirmacionBorrado').modal('show');
};

window.OcultarModalConfirmacionBorrado = function () {
    $('#modalConfirmacionBorrado').modal('hide');
};

console.log("✅ JS de modelos 3D cargado correctamente");

/* =========================================================
GRAFICO DE PEDIDOS (Chart.js)
========================================================= */
window.crearGraficoPedidos = function (
    pendientes,
    enProceso,
    enviados,
    entregados,
    cancelados
) {
    const ctx = document.getElementById('pedidosChart');

    if (!ctx) {
        console.warn("⚠ No se encontró el canvas pedidosChart");
        return;
    }

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: [
                'Pendientes',
                'En Proceso',
                'Enviados',
                'Entregados',
                'Cancelados'
            ],
            datasets: [{
                data: [
                    pendientes,
                    enProceso,
                    enviados,
                    entregados,
                    cancelados
                ],
                backgroundColor: [
                    '#f1c40f',
                    '#3498db',
                    '#9b59b6',
                    '#2ecc71',
                    '#e74c3c'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                },
                title: {
                    display: true,
                    text: 'Distribución de pedidos'
                }
            }
        }
    });
};
window.inicializarModeloBicicleta = function (rutaModeloBicicleta) {
    console.log("Inicializando modelo de bicicleta:", rutaModeloBicicleta);
    canvas = document.getElementById("renderCanvas");
    engine = new BABYLON.Engine(canvas, true);
    scene = createScene();

    BABYLON.SceneLoader.ImportMesh("", "", rutaModeloBicicleta, scene,
        function (meshes) {
            if (meshes.length > 0) {
                bicicletaMesh = meshes[0];
                bicicletaMesh.position = BABYLON.Vector3.Zero();
                bicicletaMesh.scaling = new BABYLON.Vector3(0.2, 0.2, 0.2);

                // Centramos la cámara en el modelo de la bicicleta
                var camera = new BABYLON.ArcRotateCamera("camera", -Math.PI / 2, Math.PI / 2.5, 10, bicicletaMesh.position, scene);
                camera.setTarget(bicicletaMesh.position);
                camera.attachControl(canvas, true);
                camera.lowerRadiusLimit = 5;
                camera.upperRadiusLimit = 20;

                var light = new BABYLON.HemisphericLight("light", new BABYLON.Vector3(0, 1, 0), scene);
                light.intensity = 0.7;

                var directionalLight = new BABYLON.DirectionalLight("directionalLight", new BABYLON.Vector3(-1, -2, -1), scene);
                directionalLight.intensity = 0.5;

                // Creamos un entorno simple sin necesidad de cargar archivos externos
                scene.createDefaultEnvironment({
                    createSkybox: false,
                    createGround: false,
                    cameraContrast: 2.5,
                    cameraExposure: 1
                });

                // Ajustamos la posición de la cámara después de cargar el modelo
                ajustarCamara();
                //scene.debugLayer.show();
            }
            adjustLighting();
        },
        null,
        function (scene, message, exception) {
            console.error("Error al cargar el modelo de bicicleta:", message, exception);
        }
    );

    engine.runRenderLoop(function () {
        if (scene && scene.activeCamera) {
            scene.render();
        }
    });

    window.addEventListener("resize", function () {
        engine.resize();
    });
};
//window.addEventListener("keydown", function (ev) {
//    if (ev.keyCode === 73) { // 73 es el código de la tecla 'I'
//        if (scene.debugLayer.isVisible()) {
//            scene.debugLayer.hide();
//        } else {
//            scene.debugLayer.show();
//        }
//    }
//});
function ajustarCamara() {
    if (bicicletaMesh) {
        // Calculamos el bounding box del modelo
        var boundingInfo = bicicletaMesh.getHierarchyBoundingVectors(true);
        var center = BABYLON.Vector3.Center(boundingInfo.min, boundingInfo.max);

        // Ajustamos la posición de la cámara
        if (scene.activeCamera) {
            scene.activeCamera.setTarget(center);

            // Ajustamos la distancia de la cámara basándonos en el tamaño del modelo
            var diagonal = BABYLON.Vector3.Distance(boundingInfo.min, boundingInfo.max);
            scene.activeCamera.radius = diagonal * 1.5; // Ajusta este multiplicador según sea necesario
        }
    }
}

window.actualizarModeloBicicleta = function (accesorios) {
    console.log("Actualizando modelo de bicicleta con accesorios:", accesorios);
    if (!scene) {
        console.error("La escena no está inicializada");
        return;
    }

    accesorios.forEach(function (accesorio) {
        if (accesorio.tipo !== "Bicicleta" && accesorio.rutaImagen) {
            if (accesoriosMesh[accesorio.tipo]) {
                accesoriosMesh[accesorio.tipo].dispose();
            }

            BABYLON.SceneLoader.ImportMesh("", "", accesorio.rutaImagen, scene,
                function (meshes) {
                    if (meshes.length > 0) {
                        const accesorioMesh = meshes[0];
                        accesoriosMesh[accesorio.tipo] = accesorioMesh;

                        accesorioMesh.scaling = new BABYLON.Vector3(1, 1, 1);

                        switch (accesorio.tipo) {
                            case "Canasta":
                                switch (accesorio.descripcion) {
                                    case "Chill'ka":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -33.5, 0);

                                        break;
                                    case "Mimbre Chino":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);

                                        break;
                                    case "Mimbre Chileno":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -23, -1);

                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Montura":
                                switch (accesorio.descripcion) {
                                    case "Camel":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -20, 0);
                                        break;
                                    case "Camel con Blanco":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -13.45, 0);
                                        break;
                                    case "Marron con crema":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0.9, 0);
                                        break;
                                    case "Negro":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 1, 0.1);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Bocina":
                                switch (accesorio.descripcion) {
                                    case "Plateado":
                                        accesorioMesh.position = new BABYLON.Vector3(-13.32, 0, 0);
                                        break;
                                    case "Dorado":
                                        accesorioMesh.position = new BABYLON.Vector3(-22.3, 0, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Espejos":
                                switch (accesorio.descripcion) {
                                    case "Retrovisor Negro":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);
                                        break;
                                    case "Blanco":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -6, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Mangos":
                                switch (accesorio.descripcion) {
                                    case "Negro":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);
                                        break;
                                    case "Caucho amarillo":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -13.5, 0);
                                        break;
                                    case "Caucho Naranja":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -15.8, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Timbre":
                                switch (accesorio.descripcion) {
                                    case "Metal dorado":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -2.5, 0);
                                        break;
                                    case "Metal plateado":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Asiento para Niño":
                                switch (accesorio.descripcion) {
                                    case "Azul":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -30, 0);
                                        break;
                                    case "Rojo":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -11.0, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                        }

                        accesorioMesh.parent = bicicletaMesh;
                    }

                    ajustarCamara();
                },
                null,
                function (scene, message, exception) {
                    console.error("Error al cargar el accesorio:", accesorio.tipo, message, exception);
                }
            );
        }
    });
};

window.cambiarColorBicicleta = function (color) {
    console.log("Cambiando color de la bicicleta a:", color);
    if (scene) {
        var frameMesh = scene.getMeshByName("OBJ.016_primitive2"); // Asegúrate de que este nombre coincida con el de tu modelo
        if (frameMesh) {
            console.log("Mesh encontrado:", frameMesh.name);
            if (!frameMesh.material) {
                frameMesh.material = new BABYLON.PBRMaterial("marcoMaterial", scene);
                console.log("Nuevo material PBR creado para el marco");
            }

            var colorBabylon = BABYLON.Color3.FromHexString(color);

            frameMesh.material.albedoColor = colorBabylon;
            frameMesh.material.metallic = 1; // Ajusta este valor entre 0 y 1
            frameMesh.material.roughness = 1; // Ajusta este valor entre 0 y 1
            frameMesh.material.emissiveColor = colorBabylon.scale(0.2); // Añade un poco de emisión para realzar el color

            console.log("Color del marco cambiado a", color);
        } else {
            console.log("Mesh del marco no encontrado, cambiando color de toda la bicicleta");
            if (bicicletaMesh) {
                var newMaterial = new BABYLON.PBRMaterial("bicicletaMaterial", scene);

                var colorBabylon = BABYLON.Color3.FromHexString(color);

                newMaterial.albedoColor = colorBabylon;
                newMaterial.metallic = 1; // Ajusta este valor entre 0 y 1
                newMaterial.roughness = 1; // Ajusta este valor entre 0 y 1
                newMaterial.emissiveColor = colorBabylon.scale(0.2);

                bicicletaMesh.material = newMaterial;

                if (bicicletaMesh.getChildMeshes) {
                    var childMeshes = bicicletaMesh.getChildMeshes();
                    for (var i = 0; i < childMeshes.length; i++) {
                        childMeshes[i].material = newMaterial;
                    }
                }
                console.log("Color de toda la bicicleta cambiado a", color);
            } else {
                console.error("No se pudo encontrar ni el marco ni la bicicleta completa");
            }
        }

        // Ajustar la iluminación de la escena
        adjustLighting();
    } else {
        console.error("La escena no está inicializada");
    }
};
function adjustLighting() {
    // Ajustar la luz existente
    var hemisphericLight = scene.getLightByName("light");
    if (hemisphericLight) {
        hemisphericLight.intensity = 0.7;
        hemisphericLight.diffuse = new BABYLON.Color3(1, 1, 1);
    }

    // Añadir una luz direccional si no existe
    var directionalLight = scene.getLightByName("directionalLight");
    if (!directionalLight) {
        directionalLight = new BABYLON.DirectionalLight("directionalLight", new BABYLON.Vector3(-1, -2, -1), scene);
    }
    directionalLight.intensity = 0.5;

    // Añadir una luz puntual para resaltar el modelo
    var pointLight = scene.getLightByName("pointLight");
    if (!pointLight) {
        pointLight = new BABYLON.PointLight("pointLight", new BABYLON.Vector3(0, 5, -10), scene);
    }
    pointLight.intensity = 0.3;
    // Añadir una luz ambiental si no existe
    if (!scene.getLightByName("ambientLight")) {
        var ambientLight = new BABYLON.HemisphericLight("ambientLight", new BABYLON.Vector3(0, 1, 0), scene);
        ambientLight.intensity = 0.2;
    }
}
function createScene() {
    const scene = new BABYLON.Scene(engine);
    scene.clearColor = BABYLON.Color3.FromHexString("#EDECE0");
    return scene;
}

console.log("Funciones de modelo 3D cargadas correctamente.");


