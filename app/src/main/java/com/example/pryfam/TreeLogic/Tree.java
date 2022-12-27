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
    private int uniq_ID = 0;
    private int x;
    private int y;

    private String data;




    public List<Tree> children;
    ArrayList<ImageButton> nodes = new ArrayList<>();




    public Tree(String data) {
        this.data = data;

        this.children = new ArrayList<>();
        nodes.add(init_btn());
    }

    public void addChild(Tree child) {
        this.children.add(child);
        this.nodes.add(init_btn());
        return;
    }

    public void dfs(Tree cur, int newX, int newY) {
        this.x = newX; this.y = newY;

        for (int i = 0; i < children.size(); i++) {
            if (i < children.size() / 2) {
                int stepX = -200/((cur.children.size()/2) - i);
                dfs(cur.children.get(i), this.x + stepX, this.y+80);
            }
            else {
                int stepX = 200/((cur.children.size()/2) - i);
                dfs(cur.children.get(i), this.x + stepX, this.y+80);
            }
            update_btn(nodes.get(i), this.x, this.y);
        }
        Log.d("F", String.valueOf("Node: " + cur.data));
    }

    public int GetChildsNumber() {
        return this.children.size();
    }



}

// Использование образца:
//Tree<String> tree = new Tree<String>(null, "root");
