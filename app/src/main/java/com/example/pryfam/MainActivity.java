package com.example.pryfam;


import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.ImageButton;
import android.widget.ImageView;

import com.example.pryfam.TreeLogic.Tree;

public class MainActivity extends Activity implements View.OnTouchListener {

    // Координаты root node:
    int l = 700;  // левый отступ
    int h = 0;  // верхний отступ

    // Координаты для расчёта направления свайпа:
    float baseX = 0;        float baseY = 0;        // Опорные координаты


    // Создаём объект класса tree:
    Tree family;

    // Пытаемся создать canvas на заднем плане:
    ImageView imageView;
    //

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Получаем размеры экрана:
        DisplayMetrics displayMetrics = new DisplayMetrics();
        getWindowManager().getDefaultDisplay().getMetrics(displayMetrics);
        int height = displayMetrics.heightPixels;
        int width = displayMetrics.widthPixels;
        //

        // Вешаем OnTouchListener на весь layout:
        View view = findViewById(R.id.FrameLayout); // Находим его по id.
        view.setOnTouchListener(this);              // Присваиваем его.
        // ----------------------------------------------------------------------------
        family = new Tree("root", view, this);

        family.addChild_by_key("root", new Tree("nop", view, this));
        family.addChild_by_key("root", new Tree("nop", view, this));


        family.dfs(family);
        // ----------------------------------------------------------------------------

        // Пытаемся создать canvas на заднем плане:
        imageView = findViewById(R.id.imageView);

        Bitmap bitmap = Bitmap.createBitmap(width,height, Bitmap.Config.ARGB_8888);


        // Создаём перо:
        Paint paint = new Paint();
        paint.setColor(Color.RED);
        paint.setAntiAlias(true);
        paint.setStyle(Paint.Style.STROKE);
        paint.setStrokeWidth(5);

        // Создаём канвас, т.к. только он может рисовать на нашем bitmap(оновом изображении):
        Canvas canvas = new Canvas(bitmap);

        // отрисовать на битмапе круг:
        canvas.drawCircle(50,50,25,paint);

        // Попытка сделать метод clear:
        canvas.drawRect(0, 0, width, height, clear_paint);

        imageView.setImageBitmap(bitmap);
        //
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

        // Обновление координат кнопок:
        family.update_tree_vision(family,l,h);
        // Подсчитываем:





        return true;
    }
}


