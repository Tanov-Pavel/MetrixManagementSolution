PGDMP             
    	        {            systemmetrixdb    14.5    14.4 
    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    31086    systemmetrixdb    DATABASE     k   CREATE DATABASE systemmetrixdb WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'Russian_Russia.1251';
    DROP DATABASE systemmetrixdb;
                postgres    false            �            1259    73498    disk_spaces    TABLE     �  CREATE TABLE public.disk_spaces (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    ip_address character varying(250) NOT NULL,
    name character varying(250),
    total_disk_space double precision,
    free_disk_space double precision,
    is_deleted boolean,
    create_date timestamp without time zone,
    update_date timestamp without time zone,
    delete_date timestamp without time zone
);
    DROP TABLE public.disk_spaces;
       public         heap    postgres    false            �            1259    73459    metrics    TABLE     o  CREATE TABLE public.metrics (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    ip_address character varying(250),
    cpu double precision,
    ram_total double precision,
    ram_free double precision,
    is_deleted boolean,
    create_date timestamp without time zone,
    update_date timestamp without time zone,
    delete_date timestamp without time zone
);
    DROP TABLE public.metrics;
       public         heap    postgres    false            �          0    73498    disk_spaces 
   TABLE DATA           �   COPY public.disk_spaces (id, ip_address, name, total_disk_space, free_disk_space, is_deleted, create_date, update_date, delete_date) FROM stdin;
    public          postgres    false    210           �          0    73459    metrics 
   TABLE DATA           ~   COPY public.metrics (id, ip_address, cpu, ram_total, ram_free, is_deleted, create_date, update_date, delete_date) FROM stdin;
    public          postgres    false    209   =       d           2606    73505    disk_spaces disk_spaces_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.disk_spaces
    ADD CONSTRAINT disk_spaces_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.disk_spaces DROP CONSTRAINT disk_spaces_pkey;
       public            postgres    false    210            b           2606    73464    metrics metrics_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public.metrics
    ADD CONSTRAINT metrics_pkey PRIMARY KEY (id);
 >   ALTER TABLE ONLY public.metrics DROP CONSTRAINT metrics_pkey;
       public            postgres    false    209            �      x������ � �      �      x������ � �     