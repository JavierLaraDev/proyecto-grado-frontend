/* =========================================================
   VARIABLES GLOBALES
========================================================= */
window.engine = null;
window.scene = null;
window.canvas = null;

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
