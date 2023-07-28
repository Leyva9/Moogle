#!/bin/bash

# Inicializamos variables
TEX_FILE_REPORT="report"
PDF_FILE_REPORT="$TEX_FILE_REPORT.pdf"

TEX_FILE_SLIDES="slides"
PDF_FILE_SLIDES="$TEX_FILE_SLIDES.pdf"

# Funciones:
run()
{
    # Iniciar proyecto 
    dotnet watch --project MoogleServer
}

report()
{
    cd Informe
    # Verificar si el archivo PDF ya existe
    if [ -f "$PDF_FILE_REPORT" ]; then
        echo "El archivo '$PDF_FILE_REPORT' ya existe. No es necesario compilar nuevamente."
    else
        # Compilar el archivo LaTeX
        pdflatex "$TEX_FILE_REPORT.tex"

        # Limpiar archivos auxiliares generados por LaTeX
        rm "$TEX_FILE_REPORT.aux" "$TEX_FILE_REPORT.log"

        echo "Informe generado con éxito."
    fi
    cd ..
}

slides()
{
    cd Informe
    # Verificar si el archivo PDF ya existe
    if [ -f "$PDF_FILE_SLIDES" ]; then
        echo "El archivo '$PDF_FILE_SLIDES' ya existe. No es necesario compilar nuevamente."
    else
        # Compilar el archivo LaTeX
        pdflatex "$TEX_FILE_SLIDES.tex"

        # Limpiar archivos auxiliares generados por LaTeX
        rm "$TEX_FILE_SLIDES.aux" "$TEX_FILE_SLIDES.log"

        echo "Presentación generada con éxito."
    fi
    cd ..
}

showReport()
{
    cd Informe
    if [ -f "$PDF_FILE_REPORT" ]; then
        # El archivo existe, así que podemos abrirlo
        xdg-open report.pdf 
    else
        echo "El archivo $PDF_FILE_REPORT no se encuentra. Pasaremos a generarlo..."
        report
        showReport
    fi
    cd ..
}

showSlides()
{
    cd Informe
    if [ -f "$PDF_FILE_SLIDES" ]; then
        # El archivo existe, así que podemos abrirlo
        xdg-open slides.pdf 
    else
        echo "El archivo $PDF_FILE_SLIDES no se encuentra. Pasaremos a generarlo..."
        slides
        showSlides
    fi
    cd ..
}

clean()
{
    echo "Saliendo..."
    
}

while true

do
    echo "Escriba (R) Correr el proyecto, (GR) Generar Informe, (GS) Generar presentación, (SI) Mostrar Informe, (SS) Mostrar Presentación, (C) Borrar los Archivos Innecesarios y salir"
    read entrada
    if [ "$entrada" == "R" ]; then
        run
    elif [ "$entrada" == "GR" ]; then
        report
    elif [ "$entrada" == "GS" ]; then
        slides
    elif [ "$entrada" == "SI" ]; then
        showReport
    elif [ "$entrada" == "SS" ]; then
        showSlides
    elif [ "$entrada" == "C" ]; then
        clean
        break
    else    
        echo "Escriba algo correcto establecido en los parámetros anteriores"
    fi
done

