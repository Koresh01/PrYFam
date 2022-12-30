package com.example.pryfam.TreeLogic;


import android.content.Context;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.ImageButton;
import android.widget.Toast;

import com.example.pryfam.MainActivity;
import com.example.pryfam.R;
import java.util.ArrayList;



// Класс Дерева.
public class Tree {
    private Node node;  // Хранит сокровенные данные этой ноды.

    // -----------------------------------------------------------
    ArrayList<Tree> children;   // Хранит детей.
    // -----------------------------------------------------------


    //
    private View view;
    private Context context;
    //

    public Tree(String key, View view, Context context) {   // конструктор
        this.node = new Node();
        children = new ArrayList<Tree>();

        // Чтобы создавать кнопки на нашей активности:
        this.view = view;
        this.context = context;
        // ---------------------------

        // Создаём новую кнопку для новоиспечённого члена семьи:
        this.node.btn = init_btn();
        this.node.btn.setOnClickListener(this::onClick);
        // ---------------------------

        // Устанавливаем uniqId нашей ноде:
        this.node.setKey(key);
    }

    public void addChild(String key_of, Tree new_member) {
        // Найдем узел с указанным ключом:
        Tree target = find_M_dfs(key_of, this);
        if (target != null) {
            // Выдадим ребёнку ключ:
            int new_key = (target.children.size()+1);
            new_member.node.key = target.node.key + new_key;

            // Добавим ребёнка этому узлу.
            target.children.add(new_member);
        }

    }

    public Tree find_M_dfs(String key_of, Tree cur) {  // find member by key
        if (cur.node.key.compareTo(key_of) == 0) {
            return cur;
        }
        for (int i = 0; i < cur.children.size(); i++) {
            Tree target = find_M_dfs(key_of, cur.children.get(i));  // Как только нашли эл-т с таким ключём.
            if (target != null) {
                return target;
            }
        }
        return null;
    }

    public Tree dfs(Tree cur) {  // standart dfs.
        for (int i = 0; i < cur.children.size(); i++) {
            dfs(cur.children.get(i));
        }
        Log.d("F", String.valueOf("Node: " + cur.node.key));
        return null;
    }

    public int update_tree_vision(Tree cur, int l, int h) { // Возвращает мнимую ширину поддерева.
        int N = cur.children.size();

        int dx = 160;
        int dy = 160;
        if (N >= 1) {
            for (int i = 0; i < N; i++) {
                l = update_tree_vision(cur.children.get(i), l-dx, h+dy);
            }
        }
        update_btn(cur.node.btn, l, h);
        return l;
    }

    // Функция создающая кнопку:
    public ImageButton init_btn() {
        ImageButton btn = new ImageButton(context); // Создаём объект новой кнопки.      /Этого не нужно делать в update_btn()!\

        ViewGroup FrameLayoutGroup = (ViewGroup) view.findViewById(R.id.FrameLayout);    // ViewGroup - понимается как контейнер для view элементов.
        btn.setImageResource(R.drawable.bound);
        btn.setId(View.generateViewId());   // УКАЗЫВАЕМ КНОПКЕ ID, ПРОГРАММНО! С УМОМ УКАЗЫВАЙ ID, ЕСЛИ ХОЧЕШЬ ПО ИТОГУ КОРРЕКТНЫЙ ОБРАБОТЧИК НАЖАТИЙ
        FrameLayout.LayoutParams parameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);   // Задаём новые параметры для layout.
        // Во FrameLayout Marign-ы вычисляются от левого верхнего УГЛА ЭКРАНА!!! Кнопки могут накладываться друг на друга без проблем.
        parameters.leftMargin = 20;  // Левый отступ.
        parameters.topMargin = 20;   // Отступ от верхнего края.
        btn.setLayoutParams(parameters);    // Присваиваем новые параметры нашей кнопке.
        FrameLayoutGroup.addView(btn);  // Добавляем кнопку на layout.     /Этого не нужно делать в update_btn()!\
//        btn.setOnClickListener(this::onClick);

        return btn; // Сейчас btn - локальная переменная с правильными параметрами. Её нужно присвоить в btn1, которая - глобальная!!!!!!
    }

    // Функция обновляющая кнопку, в зависимости от координат root node, а именно int l, int h:
    public ImageButton update_btn(ImageButton btn, int x, int y) {
        ViewGroup FrameLayoutGroup = (ViewGroup) view.findViewById(R.id.FrameLayout);
        FrameLayout.LayoutParams parameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        parameters.leftMargin = x;
        parameters.topMargin = y;
        btn.setLayoutParams(parameters);

        return btn;
    }

    // Обработсик нажатия на кнопку конкретной той ноды, в которой находимся.
    public void onClick(View v) {
        Toast.makeText(context, this.node.key, Toast.LENGTH_SHORT).show();
    }

    // ------------------------------------------------------------------------------------------------------
    int boolToInt(Boolean b) {
        return b.compareTo(false);
    }
}
