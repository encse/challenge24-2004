import java.net.URI
import java.nio.file.{FileSystems, Files, Paths}

import scala.annotation.tailrec
import scala.collection.JavaConverters._
import scala.collection.mutable
import scala.util.control.Breaks._

package object cmn {
  def cache[A, B](f: A => B): A => B = {
    val mp = mutable.Map[A, B]()
    a => mp.getOrElseUpdate(a, f(a))
  }

  def zipsrc() = {

    val zipPath = Paths.get("src.zip")
    Files.deleteIfExists(zipPath)
    val zipfs = FileSystems.newFileSystem(new URI(s"jar:${zipPath.toUri}"), Map("create" -> "true").asJava)

    def copy(src: String): Unit = {
      Files.walk(Paths.get(src)).iterator().asScala.foreach(path => {
        Files.copy(path, zipfs.getPath(path.toString))
      })
    }

    copy("src")
    copy(".idea")
    copy("Gcj.iml")
    zipfs.close()
  }

  def nums[T: Numeric](start: T): Stream[T] = {
    val i = implicitly[Numeric[T]]
    import i.mkNumericOps
    start #:: nums(start + i.one)
  }

  def loop(body: => Unit): Unit = {
    breakable {
      while (true) {
        body
      }
    }
  }

  implicit class For[A](val start: A) extends Traversable[A] {
    var cond: A => Boolean = { _ => true }
    var next: A => A = { a => a }

    def cond(cond: A => Boolean): For[A] = {
      this.cond = cond
      this
    }

    def next(next: A => A): For[A] = {
      this.next = next
      this
    }


    override def foreach[U](f: (A) => U): Unit = {
      breakable {
        var a = start
        while (cond(a)) {
          f(a)
          a = next(a)
        }
      }
    }
  }

  def binSearch2[T](fCheck: T => Boolean, start: T, stepFirst: T, add: (T, T) => T, twice: T => T, half: T => T, fOk: (T, T) => Boolean): (T, T) = {
    val fStart = fCheck(start)

    @tailrec
    def s1(low: T, step: T): (T, T, T) = {
      val t = add(low, step)
      if (fStart == fCheck(t)) {
        s1(t, twice(step))
      } else {
        (low, step, t)
      }

    }

    @tailrec
    def s2(low: T, step: T, high: T): (T, T) = {
      if (fOk(low, high)) {
        (low, high)
      } else {


        val halfStep = half(step)
        val t = add(low, halfStep)

        if (fStart == fCheck(t)) {
          s2(t, halfStep, high)
        } else {
          s2(low, halfStep, t)
        }

      }
    }


    (s2 _).tupled(s1(start, stepFirst))
  }

  def binSearch[T: Fractional](fCheck: T => Boolean, start: T, stepFirst: T, precision: T): (T, T) = {
    val integral = implicitly[Fractional[T]]
    val ordering = implicitly[Ordering[T]]
    val two = integral.fromInt(2)
    import integral.mkNumericOps
    import ordering.mkOrderingOps

    binSearch2[T](
      fCheck, start, stepFirst, _ + _, _ * two, _ / two, (low, high) => high - low < precision
    )
  }

  def astar[N, D: Ordering](
                             nsStart: Traversable[N],
                             fEnd: N => Boolean,
                             getNexts: N => Traversable[N],
                             getTotal: N => D,
                             enumerateSet: mutable.Set[N] => Traversable[N] = (set: mutable.Set[N]) => set
                           ): Option[(N, D)] = {

    val ordering = implicitly[Ordering[D]]

    import ordering.mkOrderingOps

    val setDone = mutable.HashSet[N]()
    val mapActive = TreeMap12[D, mutable.Set[N]]()
    val mapNtoBestTotal = mutable.HashMap[N, D]()

    def addActive(n: N, total: D): Unit = {
      mapActive.getOrElseUpdate(total, mutable.HashSet()) += n
      mapNtoBestTotal(n) = total
    }

    var ns = nsStart
    loop {
      for (n <- ns) {
        val total = getTotal(n)
        mapNtoBestTotal.get(n) match {
          case Some(oldtotal) =>
            if (oldtotal > total) {
              val setN = mapActive.get(oldtotal).get
              if (setN.size == 1)
                mapActive -= oldtotal
              else
                setN -= n
              addActive(n, total)
            }
          case None =>
            addActive(n, total)
        }

      }

      if (mapActive.isEmpty)
        return None

      val (total, setN) = mapActive.head

      setN.find(fEnd) match {
        case Some(n) =>
          return Some(n, total)
        case None =>
          for (n <- setN) {
            mapNtoBestTotal -= n
            setDone += n
          }
          mapActive -= total

          ns = enumerateSet(setN).toStream.flatMap(getNexts).filterNot(setDone.contains)
      }
    }

    throw new Exception
  }
}
