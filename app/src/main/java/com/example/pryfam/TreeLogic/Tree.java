package com.example.pryfam.TreeLogic;


import android.app.Activity;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.ImageButton;

import com.example.pryfam.R;

import java.util.ArrayList;
import java.util.List;

// Класс Дерева.
public class Tree {
    // Глобальные координаты ноды:
    public int x = 0;
    public int y = 0;
    public ImageButton btn;

    // Список деревьев-детей:
    public List<Tree> chldTree;

    public Tree(ImageButton btn) {
        this.btn = btn;
        this.chldTree = new ArrayList<>();
    }

    public void addChild(Tree child) {
        this.chldTree.add(child);
        return;
    }

    public void dfs(Tree cur, int newX, int newY) {
        this.x = newX; this.y = newY;

        for (int i = 0; i < cur.chldTree.size(); i++) {
            int stepX;
            if (i == 0) {
                stepX = -10;
            }
            else {stepX = 10;}

            dfs(cur.chldTree.get(i), this.x + stepX, this.y+10);
        }
    }
}
