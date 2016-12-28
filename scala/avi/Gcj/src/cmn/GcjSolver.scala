package cmn

import java.nio.file.{Files, Paths}

import scala.collection.JavaConverters._

abstract class Prs[T] {
  def apply(st: String): T
}

abstract class Prsrg[T](val sep: Char = ' ') extends Prs[T] {
  def prsrg(rgst: Array[String]): T

  final override def apply(st: String): T = prsrg(st.split(sep))
}

object PString extends Prs[String] {
  override def apply(st: String): String = st
}

object PInt extends Prs[Int] {
  override def apply(st: String): Int = st.toInt
}

object PLong extends Prs[Long] {
  override def apply(st: String): Long = st.toLong
}

object PDouble extends Prs[Double] {
  override def apply(st: String): Double = st.toDouble
}

object PBigDecimal extends Prs[BigDecimal] {
  override def apply(st: String): BigDecimal = BigDecimal(st)
}

object PBigInt extends Prs[BigInt] {
  override def apply(st: String): BigInt = BigInt(st)
}

case class PIndexedSeq[T](prs: Prs[T]) extends Prsrg[IndexedSeq[T]] {
  override def prsrg(rgst: Array[String]): IndexedSeq[T] = rgst.map(prs(_)).toIndexedSeq
}

case class PList[T](prs: Prs[T]) extends Prsrg[List[T]] {
  override def prsrg(rgst: Array[String]): List[T] = rgst.map(prs(_)).toList
}

case class PStream[T](prs: Prs[T]) extends Prsrg[Stream[T]] {
  override def prsrg(rgst: Array[String]): Stream[T] = rgst.map(prs(_)).toStream
}

case class PT2[T1, T2](prs1: Prs[T1], prs2: Prs[T2]) extends Prsrg[(T1, T2)] {
  override def prsrg(rgst: Array[String]): (T1, T2) = (
    prs1(rgst(0)),
    prs2(rgst.drop(1).mkString(sep.toString))
    )
}

case class PT3[T1, T2, T3](prs1: Prs[T1], prs2: Prs[T2], prs3: Prs[T3]) extends Prsrg[(T1, T2, T3)] {
  override def prsrg(rgst: Array[String]): (T1, T2, T3) = (
    prs1(rgst(0)),
    prs2(rgst(1)),
    prs3(rgst.drop(2).mkString(sep.toString))
    )
}

case class PT4[T1, T2, T3, T4](prs1: Prs[T1], prs2: Prs[T2], prs3: Prs[T3], prs4: Prs[T4]) extends Prsrg[(T1, T2, T3, T4)] {
  override def prsrg(rgst: Array[String]): (T1, T2, T3, T4) = (
    prs1(rgst(0)),
    prs2(rgst(1)),
    prs3(rgst(2)),
    prs4(rgst.drop(3).mkString(sep.toString))
    )
}

case class PT5[T1, T2, T3, T4, T5](prs1: Prs[T1], prs2: Prs[T2], prs3: Prs[T3], prs4: Prs[T4], prs5: Prs[T5]) extends Prsrg[(T1, T2, T3, T4, T5)] {
  override def prsrg(rgst: Array[String]): (T1, T2, T3, T4, T5) = (
    prs1(rgst(0)),
    prs2(rgst(1)),
    prs3(rgst(2)),
    prs4(rgst(3)),
    prs5(rgst.drop(4).mkString(sep.toString))
    )
}

trait Fetch {
  def apply[T](prs: Prs[T]): T
}

object NewLine {}

trait Out {
  def apply(xs: Any*) = xs.foreach({
    case NewLine => newLine()
    case x => write(x.toString)
  })

  def close()

  protected def write(st: String)

  protected def newLine()
}

class RefoutException(val filn: String, val irow: Int, val icol: Int)
  extends RuntimeException(s"Difference in $filn at $irow:$icol") {

}

abstract class GcjSolver() {
  def apply() = {
    println(s"Running: ${getClass.getSimpleName}")

    val Array(yyear,event,problem) = getClass.getPackage.getName.split('.')

    Files.newDirectoryStream(Paths.get(s"data/${yyear.drop(1)}/$event/$problem"), "*.in")
      .iterator()
      .asScala
      .toArray
      .sortBy(_.getFileName)
      .reverse
      .foreach(pathIn => {
        print(s"Input: ${pathIn.getFileName} ")
        val msStart = System.currentTimeMillis()
        val fpat = pathIn.toAbsolutePath.toString.dropRight(3)
        val fpatOut = s"$fpat.out"
        val fpatRefout = s"$fpat.refout"

        val pathOut = Paths.get(fpatOut)

        if (Files.exists(pathOut)) {
          val v = (-1 :: Files.newDirectoryStream(pathOut.getParent, s"${pathOut.getFileName}.v*")
            .iterator()
            .asScala
            .toList
            .map(path => path.getFileName.toString.substring(pathOut.getFileName.toString.length + 2).toInt))
            .max + 1
          Files.move(pathOut, Paths.get(s"$fpatOut.v$v"))
        }

        val bwOut = Files.newBufferedWriter(pathOut)

        val pathRefout = Paths.get(fpatRefout)
        val obrRefout = if (Files.exists(pathRefout)) Some(Files.newBufferedReader(pathRefout)) else None

        val brIn = Files.newBufferedReader(pathIn)

        solveAll(
          new Fetch {
            override def apply[T](prs: Prs[T]): T = prs(brIn.readLine())
          },
          new Out {
            var irow = 0
            var icol = 0
            var fFirst = true
            var olineRefout = nextRefoutLine

            def RefoutException: RefoutException = {
              new RefoutException(pathOut.getFileName.toString, irow + 1, icol + 1)
            }

            def nextRefoutLine = {
              obrRefout.flatMap(br => Option(br.readLine()))
            }

            def writeOut(st: String) = {
              bwOut.write(st)
              bwOut.flush()
              if (obrRefout.isDefined) {
                if (olineRefout.isEmpty)
                  throw RefoutException
                val line = olineRefout.get
                if (line.length < icol + st.length || line.substring(icol, icol + st.length) != st)
                  throw RefoutException
              }
              icol += st.length
            }

            override protected def write(st: String): Unit = {
              if (!fFirst)
                writeOut(" ")
              writeOut(st)
              fFirst = false
            }

            override protected def newLine(): Unit = {
              bwOut.write("\n")
              if (obrRefout.isDefined) {
                if (olineRefout.forall(icol != _.length))
                  throw RefoutException
              }
              fFirst = true
              irow += 1
              icol = 0
              olineRefout = nextRefoutLine
              bwOut.flush()
            }

            override def close(): Unit = {
              if (obrRefout.isDefined) {
                if (olineRefout.exists(icol != _.length))
                  throw RefoutException
                if (obrRefout.get.readLine() != null)
                  throw RefoutException
              }
            }
          }
        )

        bwOut.close()

        print(if (obrRefout.isDefined) "OK" else "UNCHECKED")
        println(s" in ${(System.currentTimeMillis()-msStart)/1000}s")
      })
    Console.flush()

  }

  def cCaseGet(fetch: Fetch) = fetch(PInt)

  def solveAll(fetch: Fetch, out: Out) = {
    val cCase = cCaseGet(fetch)
    (1 to cCase).foreach(iCase => {
      out(s"Case #$iCase:")
      solve(fetch, out)
      out(NewLine)
    })
    out.close()
  }

  def solve(fetch: Fetch, out: Out)
}

