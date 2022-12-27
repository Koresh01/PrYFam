package com.example.pryfam;


import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.ImageButton;
import com.example.pryfam.TreeLogic.Tree;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends Activity implements View.OnTouchListener {


    // Координаты root node:
    int l = 0;  // левый отступ
    int h = 0;  // верхний отступ

    // Координаты для расчёта направления свайпа:
    float baseX = 0;        float baseY = 0;        // Опорные координаты

    // Создаём объект класса tree:
    Tree family = new Tree("root");


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Добавляем в корневой эл-т дерева "family" 2ух потомков:
        Tree child_1 = new Tree("child1");
        Tree child_2 = new Tree("child2");
        // Скармливаем:
        family.addChild(child_1);
        family.addChild(child_2);


        // Добавляем 1ому потомку 2ух детей:
        Tree child_11 = new Tree("child11");
        Tree child_12 = new Tree("child12");
        // Скармливаем:
        child_1.addChild(child_11);
        child_1.addChild(child_12);


        // Проверяем
        family.dfs(family, 0, 0);
        // ----------------------------------------------------------------------------



        // Вешаем OnTouchListener на весь layout:
        View view = findViewById(R.id.FrameLayout); // Находим его по id.
        view.setOnTouchListener(this);              // Присваиваем его.
    }

    // Функция создающая кнопку:
    public ImageButton init_btn() {
        ImageButton btn = new ImageButton(this); // Создаём объект новой кнопки.      /Этого не нужно делать в update_btn()!\

        ViewGroup FrameLayoutGroup = (ViewGroup) findViewById(R.id.FrameLayout);    // ViewGroup - понимается как контейнер для view элементов.
        btn.setImageResource(R.drawable.bound);
        btn.setId(View.generateViewId());   // УКАЗЫВАЕМ КНОПКЕ ID, ПРОГРАММНО! С УМОМ УКАЗЫВАЙ ID, ЕСЛИ ХОЧЕШЬ ПО ИТОГУ КОРРЕКТНЫЙ ОБРАБОТЧИК НАЖАТИЙ
        FrameLayout.LayoutParams parameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);   // Задаём новые параметры для layout.
        // Во FrameLayout Marign-ы вычисляются от левого верхнего УГЛА ЭКРАНА!!! Кнопки могут накладываться друг на друга без проблем.
        parameters.leftMargin = l;  // Левый отступ.
        parameters.topMargin = h;   // Отступ от верхнего края.
        btn.setLayoutParams(parameters);    // Присваиваем новые параметры нашей кнопке.
        FrameLayoutGroup.addView(btn);  // Добавляем кнопку на layout.     /Этого не нужно делать в update_btn()!\

        return btn; // Сейчас btn - локальная переменная с правильными параметрами. Её нужно присвоить в btn1, которая - глобальная!!!!!!
    }

    // Функция обновляющая кнопку, в зависимости от координат root node, а именно int l, int h:
    public ImageButton update_btn(ImageButton btn, int StepX, int StepY) {
        ViewGroup FrameLayoutGroup = (ViewGroup) findViewById(R.id.FrameLayout);
        FrameLayout.LayoutParams parameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        parameters.leftMargin = StepX + l;
        parameters.topMargin = StepY + h;
        btn.setLayoutParams(parameters);

        return btn;
    }

    // Слушатель сенсорных нажатий / обработчик свайпов:
    @Override
    public boolean onTouch(View view, MotionEvent event) {
        float x = event.getX(); float y = event.getY(); // Текущие координаты
        float dx = 0;           float dy = 0;           // Вектор смещения

        // Вид сенсорного события:
        switch (event.getAction()) {
            case MotionEvent.ACTION_DOWN: // нажатие
                baseX = event.getX();
                baseY = event.getY();
                break;
            case MotionEvent.ACTION_MOVE: // движение
                x = event.getX();
                y = event.getY();
                break;
            case MotionEvent.ACTION_UP: // отпускание

                break;
            case MotionEvent.ACTION_CANCEL:
                break;
        }
        // Перерасчёт смещений:
        dx = x - baseX; dy = y - baseY;

        // Перерасчёт опорных координат, идея Ивана:
        baseX = x;
        baseY = y;

        // Смещение координат root node согласно свайпу:
        l += dx;
        h += dy;

        // Обновление координат кнопок(stepX, stepY задают смещения дочерним нодам):
        family.dfs(family, 0, 0);
        return true;
    }
}